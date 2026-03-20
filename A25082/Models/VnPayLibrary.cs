using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace A25082.Models
{
    public class VnPayLibrary
    {
        private SortedDictionary<string, string> _requestData = new SortedDictionary<string, string>();
        private SortedDictionary<string, string> _responseData = new SortedDictionary<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _requestData[key] = value;
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            var query = new StringBuilder();

            // Xây dựng query string từ SortedDictionary (tự động sắp xếp theo alphabet)
            foreach (var kv in _requestData)
            {
                if (query.Length > 0)
                    query.Append("&");
                query.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value));
            }

            var rawData = query.ToString();
            var secureHash = ComputeHmacSHA512(vnp_HashSecret, rawData);

            return baseUrl + "?" + rawData + "&vnp_SecureHash=" + secureHash;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _responseData[key] = value;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var signData = new StringBuilder();

            // Xây dựng chuỗi dữ liệu cần validate, loại bỏ vnp_SecureHash và vnp_SecureHashType
            foreach (var kv in _responseData)
            {
                if (!kv.Key.Equals("vnp_SecureHash", StringComparison.InvariantCultureIgnoreCase)
                    && !kv.Key.Equals("vnp_SecureHashType", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (signData.Length > 0)
                        signData.Append("&");
                    signData.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value));
                }
            }

            var rawData = signData.ToString();
            var checkHash = ComputeHmacSHA512(secretKey, rawData);

            // So sánh hash (case-insensitive)
            return inputHash.Equals(checkHash, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Tính toán HMAC SHA512 - Trả về lowercase (như VNPay yêu cầu)
        /// </summary>
        public static string ComputeHmacSHA512(string key, string inputData)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
                // QUAN TRỌNG: VNPay sandbox yêu cầu lowercase, không phải UPPERCASE
                var hex = BitConverter.ToString(hashValue).Replace("-", "").ToLower();
                return hex;
            }
        }
    }
}
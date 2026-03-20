using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using A25082.Models;

namespace A25082.Admin.Service
{
    public class GHNService
    {

        private const string BASE_URL = "https://online-gateway.ghn.vn/shiip/public-api";

        private const string TOKEN = "a85271c6-7bfe-11f0-ba92-52cad24c1d84";
        private const int SHOP_ID = 595712;

        // ==================== LẤY DANH SÁCH TỈNH/THÀNH PHỐ ====================
        public async Task<List<GHNProvince>> GetProvincesAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Token", TOKEN);

                var response = await client.GetAsync($"{BASE_URL}/master-data/province");
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<GHNResponse<List<GHNProvince>>>(content);
                return result?.Data ?? new List<GHNProvince>();
            }
        }

        // ==================== LẤY DANH SÁCH QUẬN/HUYỆN ====================
        public async Task<List<GHNDistrict>> GetDistrictsAsync(int provinceId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Token", TOKEN);

                var requestData = new { province_id = provinceId };
                var jsonContent = JsonConvert.SerializeObject(requestData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{BASE_URL}/master-data/district", httpContent);
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<GHNResponse<List<GHNDistrict>>>(content);
                return result?.Data ?? new List<GHNDistrict>();
            }
        }

        // ==================== LẤY DANH SÁCH PHƯỜNG/XÃ ====================
        public async Task<List<GHNWard>> GetWardsAsync(int districtId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Token", TOKEN);

                var requestData = new { district_id = districtId };
                var jsonContent = JsonConvert.SerializeObject(requestData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{BASE_URL}/master-data/ward", httpContent);
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<GHNResponse<List<GHNWard>>>(content);
                return result?.Data ?? new List<GHNWard>();
            }
        }

        // ==================== TẠO ĐƠN HÀNG GHN ====================
        // ==================== TẠO ĐƠN HÀNG GHN ====================
        public async Task<GHNOrderResponse> CreateOrderAsync(ThanhToan payment)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Token", TOKEN);
                    client.DefaultRequestHeaders.Add("ShopId", SHOP_ID.ToString());

                    // Chuẩn bị data như cũ...
                    var items = payment.ChiTietThanhToans.Select(ct => new
                    {
                        name = ct.SanPhamKemChongNang.TenKem,
                        quantity = ct.SoLuong,
                        price = (int)ct.SanPhamKemChongNang.GiaGiam
                    }).ToList();

                    int totalQuantity = payment.ChiTietThanhToans.Sum(ct => ct.SoLuong);
                    int codAmount = payment.PhuongThucThanhToan == "COD" ? (int)payment.SoTienThanhToan : 0;

                    var orderData = new
                    {
                        payment_type_id = payment.PhuongThucThanhToan == "COD" ? 2 : 1,
                        note = $"Đơn hàng #{payment.MaThanhToan}",
                        required_note = "KHONGCHOXEMHANG",
                        to_name = payment.HoTen,
                        to_phone = payment.SoDienThoai,
                        to_address = payment.DiaChi,
                        to_ward_code = payment.Phuong,
                        to_district_id = int.Parse(payment.Huyen),
                        cod_amount = codAmount,
                        content = "Đồ uống",
                        weight = totalQuantity * 200,
                        length = 20,
                        width = 20,
                        height = 15,
                        service_type_id = 2,
                        items = items
                    };

                    var jsonContent = JsonConvert.SerializeObject(orderData);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // GHI LOG REQUEST ĐỂ DEBUG
                    System.Diagnostics.Debug.WriteLine("=== GHN CREATE ORDER REQUEST ===");
                    System.Diagnostics.Debug.WriteLine($"URL: {BASE_URL}/v2/shipping-order/create");
                    System.Diagnostics.Debug.WriteLine($"Token: {TOKEN}");
                    System.Diagnostics.Debug.WriteLine($"ShopId: {SHOP_ID}");
                    System.Diagnostics.Debug.WriteLine($"Body: {jsonContent}");

                    var response = await client.PostAsync($"{BASE_URL}/v2/shipping-order/create", httpContent);
                    var content = await response.Content.ReadAsStringAsync();

                    // GHI LOG RESPONSE LUÔN, DÙ THÀNH CÔNG HAY THẤT BẠI
                    System.Diagnostics.Debug.WriteLine("=== GHN RESPONSE ===");
                    System.Diagnostics.Debug.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Response Body: {content}");

                    if (response.IsSuccessStatusCode)
                    {
                        var result = JsonConvert.DeserializeObject<GHNOrderResponse>(content);
                        return result ?? new GHNOrderResponse { Code = 400, Message = "Không parse được response thành công" };
                    }
                    else
                    {
                        // Thử parse lỗi từ GHN (thường có code và message)
                        try
                        {
                            var errorResult = JsonConvert.DeserializeObject<GHNOrderResponse>(content);
                            return new GHNOrderResponse
                            {
                                Code = errorResult?.Code ?? (int)response.StatusCode,
                                Message = errorResult?.Message ?? "Lỗi không xác định từ GHN"
                            };
                        }
                        catch
                        {
                            // Nếu không parse được JSON, trả về raw content
                            return new GHNOrderResponse
                            {
                                Code = (int)response.StatusCode,
                                Message = $"Lỗi GHN (không phải JSON): {content.Substring(0, Math.Min(content.Length, 500))}"
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // GHI LOG EXCEPTION
                System.Diagnostics.Debug.WriteLine("=== EXCEPTION KHI GỌI GHN ===");
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                return new GHNOrderResponse
                {
                    Code = 500,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }
        // ==================== TÍNH PHÍ SHIP ====================
        public async Task<int> CalculateShippingFeeAsync(int districtId, string wardCode, int weight)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Token", TOKEN);
                    client.DefaultRequestHeaders.Add("ShopId", SHOP_ID.ToString());

                    var requestData = new
                    {
                        service_type_id = 2,
                        to_district_id = districtId,
                        to_ward_code = wardCode,
                        weight = weight,
                        insurance_value = 0
                    };

                    var jsonContent = JsonConvert.SerializeObject(requestData);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{BASE_URL}/v2/shipping-order/fee", httpContent);
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<GHNFeeResponse>(content);
                    return result?.Data?.Total ?? 0;
                }
            }
            catch
            {
                return 0;
            }
        }
    }

    // ==================== CÁC CLASS MODEL CHO GHN ====================
    public class GHNResponse<T>
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class GHNProvince
    {
        [JsonProperty("ProvinceID")]
        public int ProvinceID { get; set; }

        [JsonProperty("ProvinceName")]
        public string ProvinceName { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }
    }

    public class GHNDistrict
    {
        [JsonProperty("DistrictID")]
        public int DistrictID { get; set; }

        [JsonProperty("DistrictName")]
        public string DistrictName { get; set; }

        [JsonProperty("ProvinceID")]
        public int ProvinceID { get; set; }
    }

    public class GHNWard
    {
        [JsonProperty("WardCode")]
        public string WardCode { get; set; }

        [JsonProperty("WardName")]
        public string WardName { get; set; }

        [JsonProperty("DistrictID")]
        public int DistrictID { get; set; }
    }

    public class GHNOrderResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public GHNOrderData Data { get; set; }
    }

    public class GHNOrderData
    {
        [JsonProperty("order_code")]
        public string OrderCode { get; set; }

        [JsonProperty("sort_code")]
        public string SortCode { get; set; }

        [JsonProperty("trans_type")]
        public string TransType { get; set; }

        [JsonProperty("total_fee")]
        public int TotalFee { get; set; }

        [JsonProperty("expected_delivery_time")]
        public DateTime? ExpectedDeliveryTime { get; set; }
    }

    public class GHNFeeResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public GHNFeeData Data { get; set; }
    }

    public class GHNFeeData
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("service_fee")]
        public int ServiceFee { get; set; }
    }
}
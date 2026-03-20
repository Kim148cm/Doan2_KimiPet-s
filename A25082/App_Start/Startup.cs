using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Threading.Tasks;
[assembly: OwinStartup(typeof(A25082.Startup))]

namespace A25082
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Cookie Authentication cho ứng dụng
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/DangNhapNguoiDung/Index")
            });

            //  Cookie tạm để lưu thông tin đăng nhập ngoài (Google, Facebook...)
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Google Authentication
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "100728809739-8jc5e8gofs4dhd8mnqbps68tje02fshs.apps.googleusercontent.com",
                ClientSecret = "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",
                CallbackPath = new PathString("/signin-google"),   // keep this
                                                                   // ──────────────────────────────────────── Add these lines
                AuthenticationMode = AuthenticationMode.Passive,   // usually better for Challenge()
                Provider = new GoogleOAuth2AuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        // Optional: log or add more claims if needed
                        return Task.FromResult<object>(null);
                    }
                }
            });
        }
    }
}

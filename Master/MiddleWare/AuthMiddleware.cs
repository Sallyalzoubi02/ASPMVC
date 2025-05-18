namespace Master.MiddleWare
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        // هو عبارة عن "الخطوة التالية" بالميدل ويرز. يعني بعد ما يخلص هذا الكود، يروح ينفذ الشي اللي بعده (مثلاً يروح على الكنترولر).

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next; 
            //لما نستخدم هذا الميدل وير، لازم نمررله الـ next، وهو الشي اللي لازم ينفذه بعد ما يخلص التحقق.
        }
        public async Task InvokeAsync(HttpContext context)
            //هذا هو قلب الميدل وير. كل Request يدخل على الموقع، يمر من هون.
            //🔹 HttpContext context بيعطينا كل تفاصيل الطلب الحالي(مثل عنوان الصفحة، السيشن، الكوكيز...إلخ).
        {
            var path = context.Request.Path;// هون جبنا المسار اللي المستخدم رايح عليه

            // الصفحات اللي ما بدنا نتحقق فيها من السيشن
            var allowedPaths = new[]
            {
                "/", "/Home", "/Shop","/About","/recycling","/contact","/JoinUs", "/Cart", "/Home/Sign", "/Register","/Login"
            };

            bool isPublic = allowedPaths.Any(p => path.StartsWithSegments(p));

            if (!isPublic)
            {
                var isLoggedIn = context.Session.GetString("logged"); // طالما هي بتكون "true"
                if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn == "false")

                {
                    context.Response.Redirect("/Home/Sign?formType=login");
                    return;
                }
            }

            await _next(context);
        }

    }
}

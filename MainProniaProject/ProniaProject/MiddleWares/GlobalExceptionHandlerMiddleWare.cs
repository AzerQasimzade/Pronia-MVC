namespace ProniaProject.MiddleWares
{
    public class GlobalExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                context.Response.Redirect($"/error?error={e.Message}");
            }
        }
    }
}

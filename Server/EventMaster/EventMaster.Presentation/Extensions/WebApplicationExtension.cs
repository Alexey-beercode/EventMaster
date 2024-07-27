namespace EventMaster.Extensions;

public static class WebApplicationExtension
{
    public static void AddSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
    public static void AddApplicationMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCors(builder =>
        {
            builder.WithOrigins("https://localhost:44315") 
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowCredentials();
        }); 
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
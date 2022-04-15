using Microsoft.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor().AddLogging().AddHttpClient();

//headers propagation
//Microsoft.AspNetCore.HeaderPropagation package
builder.Services.AddHttpContextAccessor().AddLogging().AddHttpClient("").AddHeaderPropagation();
builder.Services.AddHeaderPropagation(options =>
{
    options.Headers.Add("X-LOOP-DETECT");
});

builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//headers propagation
app.UseHeaderPropagation();

app.UseRouting();

app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();

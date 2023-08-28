using Microsoft.AspNetCore.Authentication;
using mockTecoAPI;
using System.Net;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Basic", policy =>
    {
        policy.AuthenticationSchemes.Add("Basic");
        policy.RequireAuthenticatedUser();
    });
});
var certgen = new CertGen();
var certificate = certgen.GenerateSelfSignedCertificate();

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps(certificate);
    });
    options.ListenAnyIP(80);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCertificateForwarding();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

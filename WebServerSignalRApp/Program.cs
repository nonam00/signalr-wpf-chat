using WebServerSignalRApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();

app.MapHub<ChatHub>("/chat");

app.Run();

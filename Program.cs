using MassTransit;
using DataService;
using NotificationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBusinessAccess, BusinessAccessLayer>();
builder.Services.AddScoped<IDataAccess, ServiceDataAccess>();
builder.Services.AddScoped<IDataContext, PostgresDataContext>();
builder.Services.AddScoped<INotification, ServiceNotification>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotificationServiceConsumer>();

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:host"], builder.Configuration["RabbitMq:virtualHost"], h =>
        {
            h.Username(builder.Configuration["RabbitMq:user"]);
            h.Password(builder.Configuration["RabbitMq:password"]);
        });
        cfg.ConfigureEndpoints(context);
    });

    x.AddRequestClient<DataServiceContract>();
    x.AddRequestClient<NotificationServiceContract>();
    
}).AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

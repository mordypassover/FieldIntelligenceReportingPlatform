using Elastic.Clients.Elasticsearch;
using ReportApi.Middlewares;
using ReportApi.repositorys;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settings = new ElasticsearchClientSettings(
    new Uri(builder.Configuration["ElasticSearch:Endpoint"])
);
var elasticSearchClient = new ElasticsearchClient(settings);
builder.Services.AddSingleton(elasticSearchClient);

builder.Services.AddScoped<IElasticRepository, ElasticRepository>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

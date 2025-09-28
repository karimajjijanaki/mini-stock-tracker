using StockTrackerAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// In-memory storage
var stocks = new List<StockHolding>();

// Mock market price function
decimal GetMockMarketPrice(string symbol) => symbol switch
{
    "AAPL" => 180,
    "MSFT" => 320,
    "GOOG" => 2700,
    _ => 100
};

// POST /api/stocks - Add a stock
app.MapPost("/api/stocks", (AddStockRequest req) =>
{
    var existing = stocks.FirstOrDefault(s => s.Symbol == req.Symbol);
    if (existing != null)
    {
        existing.TotalQuantity += req.Quantity;
        existing.TotalCost += req.Quantity * req.Price;
    }
    else
    {
        stocks.Add(new StockHolding
        {
            Symbol = req.Symbol,
            TotalQuantity = req.Quantity,
            TotalCost = req.Quantity * req.Price
        });
    }
    return Results.Ok();
});

// PUT /api/stocks/{symbol} - Update quantity only
app.MapPut("/api/stocks/{symbol}", (string symbol, UpdateQuantityRequest req) =>
{
    var stock = stocks.FirstOrDefault(s => s.Symbol == symbol);
    if (stock == null) return Results.NotFound();
    stock.TotalQuantity = req.Quantity;
    return Results.Ok();
});

// GET /api/stocks - List all stocks with computed fields
app.MapGet("/api/stocks", () =>
{
    return stocks.Select(s => new
    {
        symbol = s.Symbol,
        quantity = s.TotalQuantity,
        averageBuyPrice = s.TotalCost / s.TotalQuantity,
        totalInvested = s.TotalCost
    });
});

// GET /api/portfolio/value - Return total portfolio value
app.MapGet("/api/portfolio/value", () =>
{
    var totalValue = stocks.Sum(s => s.TotalQuantity * GetMockMarketPrice(s.Symbol));
    return new { totalValue };
});

app.Run();

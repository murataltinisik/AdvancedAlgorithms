using CourierDispatcher.Enums;
using CourierDispatcher.Models;
using CourierDispatcher.Services.Abstraction;
using CourierDispatcher.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Path = CourierDispatcher.Models.Path;

var services = new ServiceCollection();

services.AddScoped<ICommandTypeService, CommandTypeService>();

ICommandTypeService commandTypeService = new CommandTypeService();

int numberOfDistricts = 0;
List<Path> paths = new List<Path>();
List<Order> orders = new List<Order>();
List<Courier> couriers = new List<Courier>();
List<District> districts = new List<District>();

// Districts
districts.Add(new District { No = 1 });
districts.Add(new District { No = 2 });
districts.Add(new District { No = 3 });
districts.Add(new District { No = 4 });
districts.Add(new District { No = 5 });

// Paths
paths.Add(new Path { A = 1, B = 2, Minute = 10 });
paths.Add(new Path { A = 1, B = 3, Minute = 15 });
paths.Add(new Path { A = 2, B = 3, Minute = 8 });
paths.Add(new Path { A = 2, B = 4, Minute = 12 });
paths.Add(new Path { A = 3, B = 4, Minute = 7 });
paths.Add(new Path { A = 3, B = 5, Minute = 20 });
paths.Add(new Path { A = 4, B = 5, Minute = 9 });

// Couriers
couriers.Add(new Courier { No = 1, DistrictId = 1, Status = true });
couriers.Add(new Courier { No = 2, DistrictId = 3, Status = true });
couriers.Add(new Courier { No = 3, DistrictId = 5, Status = true });
couriers.Add(new Courier { No = 4, DistrictId = 2, Status = false });
couriers.Add(new Courier { No = 5, DistrictId = 4, Status = false });

do
{
    Console.Write("Enter, Command: ");
    string? raw = Console.ReadLine();

    if (string.IsNullOrEmpty(raw))
    {
        Console.WriteLine("Please, enter your command!");
        continue;
    }

    string[] rawArray = raw.Trim().Split(" ");
    string commandType = rawArray[0];

    if (!commandTypeService.IsValidType(commandType))
    {
        Console.WriteLine($"Invalid Command Type, please try again: {CommandType.Path}, {CommandType.Order}, {CommandType.Courier}, {CommandType.City}");
        continue;
    }
    
    if (!commandTypeService.IsValidProperties(raw, commandType))
    {
        Console.WriteLine("Invalid Command Properties!");
        Console.WriteLine($"{CommandType.City} numberOfDistrict=5");
        Console.WriteLine($"{CommandType.Path} a=1 b=2 minute=10");
        Console.WriteLine($"{CommandType.Courier} no=5 district=5");
        Console.WriteLine($"{CommandType.Order} time=10 restaurant=1 customer=2");
        continue;
    }

    Path path = new Path();
    Order order = new Order();
    Courier courier = new Courier();

    foreach (var rawString in rawArray)
    {
        var property = rawString.Split("=");

        for (int i = 0; i < property.Length - 1; i++)
        {
            string fieldValue = property[1];
            string fieldName = property[0].ToLower();

            if (commandType == CommandType.City)
            {
                switch (fieldName)
                {
                    case "numberofdistrict":
                        int.TryParse(fieldValue, out int val);
                        numberOfDistricts = val;
                        break;
                }
            }
            else if (commandType == CommandType.Path)
            {
                switch (fieldName)
                {
                    case "a":
                        int.TryParse(fieldValue, out int a);
                        path.A = a;
                        break;
                    case "b":
                        int.TryParse(fieldValue, out int b);
                        path.B = b;
                        break;
                    case "minute":
                        int.TryParse(fieldValue, out int minute);
                        path.Minute = minute;
                        break;
                }
            }
            else if (commandType == CommandType.Order)
            {
                switch (fieldName)
                {
                    case "time":
                        int.TryParse(fieldValue, out int time);
                        order.Time = time;
                        break;
                    case "restaurant":
                        int.TryParse(fieldValue, out int restaurant);
                        order.Restaurant = restaurant;
                        break;
                    case "customer":
                        int.TryParse(fieldValue, out int customer);
                        order.Customer = customer;
                        break;
                }
            }
            else if (commandType == CommandType.Courier)
            {
                switch (fieldName)
                {
                    case "no":
                        int.TryParse(fieldValue, out int no);
                        courier.No = no;
                        break;
                    case "district":
                        int.TryParse(fieldValue, out int districtId);
                        courier.DistrictId = districtId;
                        break;
                }
            }
        }
    }

    switch (commandType)
    {
        case CommandType.Path:
            if (districts.Any(x => x.No != path.A) || districts.Any(x => x.No != path.B))
            {
                Console.WriteLine("Path: Not found district by A or B");
                continue;
            }

            paths.Add(path);
            break;
        case CommandType.City:
            for (int i = 0; i < numberOfDistricts - 1; i++)
                districts.Add(new District() { No = i + 1 });
            break;
        
        case CommandType.Order:
        {
            List<Courier> availableCouriers = couriers
                .Where(x => x.Status)
                .ToList();

            if (!availableCouriers.Any())
            {
                Console.WriteLine("Cancelled order: No available couriers.");
                continue;
            }

            if (!districts.Any(x => x.No == order.Restaurant) ||
                !districts.Any(x => x.No == order.Customer))
            {
                Console.WriteLine("Order: Invalid restaurant or customer district.");
                continue;
            }

            Courier? selectedCourier = null;
            int minimumMinutes = int.MaxValue;

            foreach (var availableCourier in availableCouriers)
            {
                int totalMinutes = FindCourier(
                    availableCourier.DistrictId,
                    order.Restaurant,
                    order.Customer
                );

                if (totalMinutes != -1 && totalMinutes < minimumMinutes)
                {
                    minimumMinutes = totalMinutes;
                    selectedCourier = availableCourier;
                }
            }

            if (selectedCourier is null)
            {
                Console.WriteLine("No available route found for this order.");
                continue;
            }

            int orderId = orders.Count + 1;
            order.Id = $"S{orderId}";

            orders.Add(order);

            selectedCourier.Status = false;

            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Order Id: {order.Id}");
            Console.WriteLine($"Selected Courier: {selectedCourier.No}");
            Console.WriteLine($"Courier District: {selectedCourier.DistrictId}");
            Console.WriteLine($"Restaurant District: {order.Restaurant}");
            Console.WriteLine($"Customer District: {order.Customer}");
            Console.WriteLine($"Total Minutes: {minimumMinutes}");
            Console.WriteLine("--------------------------------");

            break;
        }
        case CommandType.Courier:
            if (districts.Any(x => x.No != courier.DistrictId))
            {
                Console.WriteLine("Courier: Not found district by district id");
                continue;
            }
            
            couriers.Add(courier);
            break;
    }
}while(true);

int FindCourier(int courierDistrictNo, int restaurantDistrictNo, int customerDistrictNo)
{
    int FindShortestPath(int startDistrict, int targetDistrict)
    {
        Dictionary<int, int> distances = new Dictionary<int, int>();
        HashSet<int> visited = new HashSet<int>();

        foreach (var district in districts)
        {
            distances.Add(
                district.No,
                district.No == startDistrict ? 0 : int.MaxValue
            );
        }

        while (visited.Count < districts.Count)
        {
            int currentDistrict = distances
                .Where(x => !visited.Contains(x.Key))
                .OrderBy(x => x.Value)
                .First().Key;

            if (currentDistrict == targetDistrict)
                return distances[currentDistrict];

            if (distances[currentDistrict] == int.MaxValue)
                break;

            visited.Add(currentDistrict);

            var connectedPaths = paths.Where(x =>
                x.A == currentDistrict || x.B == currentDistrict
            ).ToList();

            foreach (var path in connectedPaths)
            {
                int neighborDistrict = path.A == currentDistrict
                    ? path.B
                    : path.A;

                if (visited.Contains(neighborDistrict))
                    continue;

                int newDistance = distances[currentDistrict] + path.Minute;

                if (newDistance < distances[neighborDistrict])
                {
                    distances[neighborDistrict] = newDistance;
                }
            }
        }

        return int.MaxValue;
    }

    int courierToRestaurant = FindShortestPath(
        courierDistrictNo,
        restaurantDistrictNo
    );

    int restaurantToCustomer = FindShortestPath(
        restaurantDistrictNo,
        customerDistrictNo
    );

    if (courierToRestaurant == int.MaxValue ||
        restaurantToCustomer == int.MaxValue)
        return -1;

    return courierToRestaurant + restaurantToCustomer;
}
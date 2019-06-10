using System;
using System.Collections.Generic;
using System.Linq;
using DataModelCore.DataContexts;
using DataModelCore.ObjectModel.Primitives;
// ReSharper disable LocalizableElement

namespace ServiceInitializer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(@"Initializing data context...");
            CompetitionContext context;
            try
            {
                context = new CompetitionContext();
                Console.WriteLine(@"Done");
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Error occured: " + e.Message);
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Available commands:");
            Console.WriteLine("\timport - write new services to database");
            Console.WriteLine("\tlist - get list of services in database");
            Console.WriteLine("\tdelete [serviceId] - delete service from database");
            while (true)
            {
                Console.Write(">>> ");
                var line = Console.ReadLine();
                if (line == null) return;

                var lineData = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var command = lineData[0];
                switch (command)
                {
                    case "list":
                        var services = (from service in context.Services
                            select new[] {service.ServiceId, service.Description}).ToList();
                        Console.WriteLine($"Total entries: {services.Count}");
                        if (services.Count != 0)
                        {
                            Console.WriteLine("ServiceId\tDescription");
                            Console.WriteLine("------------------------");
                            foreach (var service in services)
                                Console.WriteLine($"{service[0]}\t{service[1]}");
                        }
                        break;
                    case "delete":
                        var serviceIdToDelete = lineData[1];
                        var serviceToDelete =
                            (from service in context.Services
                                where service.ServiceId == serviceIdToDelete
                                select service).FirstOrDefault();
                        if (serviceToDelete != null)
                        {
                            Console.WriteLine($"Deleting service {serviceToDelete.ServiceId}");
                            context.Services.Remove(serviceToDelete);
                            context.SaveChanges();
                            Console.WriteLine("Completed");
                        }
                        else
                        {
                            Console.WriteLine($"Cannot find service with id {serviceIdToDelete}");
                        }
                        break;
                    case "import":
                        Console.WriteLine("Enter services in folowing format: [serviceId] [description]");
                        Console.WriteLine("Write \"end\" to complete import and flush changes");
                        var newServices = new List<Service>();
                        while (true)
                        {
                            var newService = Console.ReadLine();
                            if (newService == null) break;
                            if (newService == "end") break;

                            var newServiceData = newService.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                            var description = newServiceData.Skip(1).Aggregate((a, b) => a + ' ' + b);
                            newServices.Add(
                                new Service {ServiceId = newServiceData[0], Description = description});
                        }
                        Console.WriteLine($"Writing {newServices.Count} new entries...");
                        context.Services.AddRange(newServices);
                        context.SaveChanges();
                        Console.WriteLine("Completed");
                        break;
                    default:
                        Console.WriteLine("Unknown command. Use one of the following:");
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("\timport - write new services to database");
                        Console.WriteLine("\tlist - get list of services in database");
                        Console.WriteLine("\tdelete [serviceId] - delete service from database");
                        break;
                }
            }

        }
    }
}
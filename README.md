# MicroServices

## API
- .Net 7
- CQRS Pattern + mediatR
- MassTransit + RabbitMQ
## Service
- .Net 7
- MassTransit + RabbitMQ
- Entity Framework (InMemory mode)
- Bogus

## Usage
Runs a instance of RabbitMQ in the default port 5672. (Both API and Service will use the default user guest to access it).
Then, use .NET cli to run the ChatAPI and the CoordinatorService:
```sh
dotnet build
dotnet run
```
The API will run in port 5091 with swagger.

#### Improvements
I thought about some improvements that could be done if I have more free time to work on it:
1. Docker would be a perfect fit for it using docker compose.
2. Using the MassTransit requests rather than send a message using the bus publish. In that way, we have a kind of request/response.
3. Improve the way that the service handles the information. The requirements were a little bit confuse to me when I started working on it. That's why I am using a database instead of handle everything in memory. In general, using a database is a good way to keep some data about what is happening, but it should have a proper service for it.


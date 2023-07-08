# Playing with Wolverine and Marten

In this repository, I have started putting together some examples of using **Wolverine** and **Marten** during my learning process.

Both Wolverine and Marten are part of the "Critter Stack" family of 👤[JasperFx tools](https://github.com/JasperFx), they are [Open Source projects](https://jeremydmiller.com/open-source-projects) 📓*Jeremy Miller*.

- **Wolwerine**: Next generation Mediator and MessageBus, it is all about professional messaging
- **Marten**: Allows developers to use the PostgreSQL database as a document database and a fully-featured EventStore

They can be used individually as they serve their own purpose, but they can be integrated together to open a new door for neat features (durable Inbox and Outbox messaging).

##### The solution has the following projects

###### WolverineHttpWithMarten

- Web API using WolverineFx.Http for managing Http endpoint routing
- Using Marten to store a Product entity and DurableInbox for local queue messages in a transaction
- Apply fluent validation middleware and a custom to load Product entity by id

###### Worker.Ping

- Worker service with a few BackgroundService
- Sending in-process ping-pong messages via local queue
- Sending PingMessage via RabbitMq and listening for PongMessage (from Worker.Pong)
- Sending RequestMessage via RabbitMq and require a ResponseMessage via RabbitMq response-queue

###### Worker.Pong

- Listening for PingMessage and send a PongMessage via RabbitMq
- Getting RequestMessage via RabbitMq and send back a ResponseMessage via RabbitMq response-queue

###### WolverineHttpWithMarten.UnitTest

- An example of unit testing our work and mocking 2 main components
  - IMessageBus or IMessageContext for Wolverine with the built-in TestMessageContext
  - IDocumentSession for Marten with any framework (Moq, NSubstitute)

###### WolverineHttpWithMarten.IntegrationTest

- An example of integration testing the Web API using
  - [Alba](https://jasperfx.github.io/alba) to spin up Web API in memory using the built-in TestServer
  - [Testcontainers](https://github.com/19balazs86/PlayingWithTestContainers) to get a new PostgresDB for our test and throw it away at the end
- You can disable external Wolverine transports to have an isolated environment
- You can run HTTP requests against you API
- Wolverine provides you with a built-in mechanism called TrackedSession that ensures certain events/messages are sent out
- You can trigger any handlers by invoking them from outside and simulate that you received a message from RabbitMQ
- Using a feature of Marten, you can easily wipe out all data and initialize it again before running all the tests
- All of these features are extremely good to work with

###### EventSourcingApi

- Web API with a few endpoints to operate on Events
- Example #1: Counter - that we can increase / decrease
  - Simulate parallels access to an event-stream and using AppendExclusive to lock the actual stream
  - Apply an inline projection
  - Apply an async projection (handle an intermittent exception with retry)

###### SagaApi

- Keeping it simple, and using Web API endpoints to manage Saga state instead of external transports
- EmailConfirmation process as an example


---

> Personal opinion:
>
> - Both Wolverine and Marten are more than awesome, I enjoy working with them
> - They looks easy to use, but they can become somewhat larger animals as you discover the itsy-bitsy details
> - The documentation is very good and sufficient length to keep you occupied
> - Unit and Integration tests are like a dream with Alba
> - Wolverine is not just yet another messaging framework, it also encourages you to think differently while building a system

---

##### Resources - Wolwerine and Marten

- [Wolverine as Mediator for Amazon SQS, Inbox and Outbox using Marten](https://youtu.be/YlG3bnJ7yCc) 📽*18m -* *Raw Coding* 

##### Resources - Wolwerine

- [Wolverine](https://wolverine.netlify.app) 📓*Documentation*
  - [Sample projects](https://wolverine.netlify.app/guide/samples.html) | [Testing](https://wolverine.netlify.app/guide/testing.html) | [Saga](https://wolverine.netlify.app/guide/durability/sagas.html)
- [Custom error handling middleware for Wolverine.Http](https://jeremydmiller.com/2023/06/28/custom-error-handling-middleware-for-wolverine-http) 📓*Jeremy Miller - Apply middleware on relevant routes, change OpenAPI description*
- [Customizing return value behavior](https://jeremydmiller.com/2023/07/05/customizing-return-value-behavior-in-wolverine-for-profit-and-fun) 📓*Jeremy Miller*
- [Your next messaging library](https://youtu.be/EGwepoGG0CM) 📽️*1h13min - JetBrains - Jeremy Miller*

##### Resources - Marten

- [MartenDB](https://martendb.io) 📓*Documentation*
- [Marten - Document DB on PostgreSQL](https://youtu.be/lgd_HxGBa-U) 📽️*20m - Raw Coding*
- Event Sourcing
  - [Marten as an event-store](https://martendb.io/events) 📓*Documentation*
  - [Wolverine Aggregate Handlers and Event Sourcing](https://wolverine.netlify.app/guide/durability/marten/event-sourcing.html) 📓*Documentation*
  - The ultimate resources [EventSourcing.NetCore](https://github.com/oskardudycz/EventSourcing.NetCore) 👤*Oskar Dudycz*
  - [Event-driven projections explained](https://event-driven.io/en/projections_in_marten_explained) 📓*Oskar Dudycz*
    - Incident management example using: SingleStreamProjection, EventProjection, MultiStreamProjection, IAggregateGrouper
  - [Practical Event Sourcing with Marten](https://youtu.be/jnDchr5eabI) 📽️*1hour - Oskar Dudycz - NDC Oslo 2023*
  - [Marten - Event Storage on PostgreSQL](https://youtu.be/z0DLQ6MDH5A) 📽️*22m - Raw Coding*
  - [Keep your streams short](https://www.eventstore.com/blog/keep-your-streams-short-temporal-modelling-for-fast-reads-and-optimal-data-retention) 📓*Oskar Dudycz - EventStore*
# Playing with Wolverine and Marten

In this repository, I have started putting together some examples of using **Wolverine** and **Marten** during my learning process.

Both Wolverine and Marten are part of the "Critter Stack" family of 👤[JasperFx tools](https://github.com/JasperFx), they are [Open Source projects](https://jeremydmiller.com/open-source-projects) 📓*Jeremy Miller*.

- **Wolwerine**: Next generation Mediator and MessageBus, it is all about professional messaging
- **MartenDB**: Transactional DocumentDB and EventStore using PostgreSQL

They can be used individually as they serve their own purpose, but they can be integrated together to open a new door for neat features (durable Inbox and Outbox messaging).

##### The solution has the following projects

###### WolverineHttpWebAPI

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

###### WolverineHttpWebAPI.UnitTest

- An example of unit testing our work and mocking 2 main components
  - IMessageBus or IMessageContext for Wolverine with the built-in TestMessageContext
  - IDocumentSession for Marten with any framework (Moq, NSubstitute)

> Personal opinion:
>
> - Both Wolverine and Marten are more than awesome. I enjoy working with them. I can imagine Wolverine as a must have in most projects
> - They looks easy to use, but they can become somewhat larger animals as you discover the itsy-bitsy details
> - The documentation is very good and sufficient length to keep you occupied
> - Unit and Integration tests are like a dream with Alba (also from 'Critter Stack' family)

##### Resources - Wolwerine and MartenDB

- [Wolverine as Mediator for Amazon SQS, Inbox and Outbox using Marten](https://youtu.be/YlG3bnJ7yCc) 📽*18m -* *Raw Coding* 

##### Resources - Wolwerine

- [Wolverine](https://wolverine.netlify.app) 📓*Offical documentation*
  - [Sample projects](https://wolverine.netlify.app/guide/samples.html)
  - [Tesing](https://wolverine.netlify.app/guide/testing.html)
- [Custom error handling middleware for Wolverine.Http](https://jeremydmiller.com/2023/06/28/custom-error-handling-middleware-for-wolverine-http) 📓*Jeremy Miller - Apply middleware on relevant routes, change OpenAPI description*
- [Your next messaging library](https://youtu.be/EGwepoGG0CM) 📽️*1h13min - JetBrains - Jeremy Miller*

##### Resources - MartenDB

- [MartenDB](https://martendb.io) 📓*Offical documentation*
- [Marten - Document DB on PostgreSQL](https://youtu.be/lgd_HxGBa-U) 📽️*20m - Raw Coding*
- [Marten - Event Storage on PostgreSQL](https://youtu.be/z0DLQ6MDH5A) 📽️*22m - Raw Coding*
- [Practical Event Sourcing with Marten](https://youtu.be/jnDchr5eabI) 📽️*1hour - Oskar Dudycz- NDC Oslo 2023*
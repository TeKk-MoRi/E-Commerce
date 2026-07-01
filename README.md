# E-Commerce Backend Platform

A work-in-progress e-commerce backend built with **.NET 10**, focusing on real backend architecture, authentication, authorization, service boundaries, and microservices-oriented design.

The project is designed as a learning and portfolio project, but follows practical enterprise patterns such as separated services, CQRS, DDD-style domain modeling, Keycloak-based identity, PostgreSQL persistence, and permission-based authorization.

## Current Architecture

The solution is organized as a microservices-oriented monorepo. Each service is intended to own its own API, application layer, domain layer, infrastructure layer, database, and migrations.

Current services:

* **Catalog Service** — product/catalog management
* **IdentityAccess Service** — authentication integration, authorization, role-permission management
* **Gateway** — planned YARP API Gateway
* **Basket Service** — planned
* **Ordering Service** — planned

Shared technical code is placed in **BuildingBlocks** and distributed locally as a NuGet package.

## Implemented So Far

### IdentityAccess

* Login with Keycloak
* Register users through Keycloak
* Refresh token flow
* Logout flow
* JWT validation
* Keycloak role claim transformation
* Permission entities
* Role-permission mapping
* PostgreSQL database for IdentityAccess
* EF Core migrations and seed data
* Permission-based authorization policies
* Role permission read/update endpoints
* User-role assignment/unassignment flow through Keycloak Admin API

### Catalog

* Product domain model
* Product validation rules
* EF Core persistence
* PostgreSQL database
* Service-separated project structure

## Authorization Design

Authentication is handled by **Keycloak**.

Authorization is handled by the application using permission policies.

Keycloak owns:

* Users
* Passwords
* Tokens
* User-to-role assignment

IdentityAccess owns:

* Application permissions
* Role-to-permission mapping
* Permission-based access checks

Example permission model:

```text
admin    -> identity.roles.manage
admin    -> identity.users.manage
admin    -> catalog.products.manage

customer -> catalog.products.view
customer -> ordering.orders.view-own
```

This keeps authentication external while keeping business permissions inside the application.

## Technology Stack

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* Keycloak
* MediatR
* CQRS
* DDD-style domain modeling
* YARP API Gateway planned
* RabbitMQ / MassTransit planned
* Docker for local infrastructure

## Project Goals

The goal of this project is to build a realistic backend system with:

* service boundaries
* independent databases per service
* permission-based authorization
* API Gateway integration
* asynchronous messaging
* saga-based order processing
* testable application and domain logic

## Planned Next Steps

* Complete IdentityAccess admin workflows
* Add YARP API Gateway
* Route Catalog and IdentityAccess through Gateway
* Add Basket service
* Add Ordering service
* Add RabbitMQ/MassTransit integration events
* Add saga orchestration for checkout/order flow
* Add more automated tests

## Status

This project is actively under development.

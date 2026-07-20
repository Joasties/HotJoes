# HJ-005 - Coding Standards

| Document ID | HJ-005 |
| --- | --- |
| Document Title | Coding Standards |
| Version | 1.0 |
| Status | Draft |
| Classification | Architecture |
| Owner | Project Architecture |
| Last Updated | 21 July 2026 |

## Revision History

| Version | Date | Description |
| --- | --- | --- |
| 0.1 | 16 July 2026 | Previous draft. |
| 1.0 | 17 July 2026 | Applied the standard HotJoes document metadata, revision history, related documents and numbered heading structure. Coding standards unchanged. |

## Related Documents

| Document ID | Title | Status |
| --- | --- | --- |
| HJ-001 | Project Vision | Draft |
| HJ-002 | Architectural Principles | Draft |
| HJ-003 | Ubiquitous Language Guide | Draft |
| HJ-004 | Vendor Domain Models | Draft |
| HJ-006 | Testing Strategy and Standards | Draft |
| HJ-007 | Enforcement Strategy | Draft |
| HJ-008 | AI Roles and Responsibilities | Draft |

## 1. Purpose

These standards define how code in the HotJoes system should be written, structured, reviewed, and maintained.

Their purpose is to:

- make the codebase understandable to another competent developer;
- make system behaviour explicit;
- prevent accidental coupling between domains;
- reduce hidden framework behaviour;
- support reliable automated testing;
- make architectural boundaries visible in the code;
- provide objective rules that can be enforced through tooling and code review.

These standards are not intended to replace engineering judgement. They establish the default position. Any deliberate exception must have a clear technical justification.

## 2. Core Engineering Principles

### 2.1 Code Is Written for Humans First

Code must be understandable without requiring the reader to reverse-engineer the developer’s intent.

Names, structure, types, and control flow should communicate what the code does and why it exists.

Code should be commented where comments improve clarity, particularly when:

- a business rule is not obvious;
- a technical decision has a non-obvious reason;
- an implementation contains an unavoidable workaround;
- behaviour depends on an external constraint;
- removing apparently unnecessary code would introduce a defect.

Comments must explain intent, constraints, or reasoning. They should not merely repeat the code.

Poor:

```csharp
// Increment count
count++;
```

Useful:

```csharp
// Menu versions remain immutable once published, so a new version
// is created rather than updating the existing record.
```

A comment must not be used to compensate for unclear code that could reasonably be rewritten.

### 2.2 Behaviour Must Be Explicit

Important system behaviour must be visible in normal code flow.

Avoid designs where behaviour is triggered indirectly through:

- property setters;
- implicit persistence hooks;
- hidden global state;
- reflection-heavy conventions;
- broad framework interception;
- untraceable event registration;
- database behaviour that is not represented in the application configuration.

A developer should be able to follow a request from its entry point through its application flow without needing to know undocumented framework conventions.

### 2.3 No Magic Black Boxes

Frameworks may reduce repetitive code, but they must not conceal business behaviour, ownership rules, data changes, or control flow.

Entity Framework, dependency injection, middleware, message brokers, validation libraries, mapping libraries, and other infrastructure must be configured explicitly enough that their behaviour can be understood and tested.

The phrase “the framework handles it” is not an adequate design explanation.

The relevant questions are:

- What behaviour does the framework perform?
- Where is that behaviour configured?
- Which layer owns the decision?
- How is it tested?
- What happens when it fails?

### 2.4 Prefer Simplicity Over Abstraction

Do not introduce abstractions merely because they may theoretically be useful later.

An abstraction should exist because it currently provides one or more of the following:

- a meaningful domain boundary;
- isolation from infrastructure;
- removal of genuine duplication;
- improved testability;
- support for multiple real implementations;
- clearer ownership or responsibility.

Do not create interfaces for every class by default.

Do not create generic base classes, generic repositories, factories, providers, managers, or helper layers without a concrete need.

Three understandable classes are preferable to one highly generic abstraction that obscures behaviour.

### 2.5 Make Invalid States Difficult to Represent

Use types and constructors to prevent invalid data from entering the domain.

Domain objects must not depend entirely on calling code remembering to validate them correctly.

Examples include:

- strongly typed identifiers;
- value objects;
- required constructor parameters;
- controlled state transitions;
- explicit validation before creation.

Primitive types should not be used where doing so makes unrelated concepts interchangeable.

## 3. Solution and Project Structure

### 3.1 Existing Structure

The solution currently follows this structure:

```csharp
HotJoes.sln

src/
  HotJoes.Api
  HotJoes.Application.Vendor
  HotJoes.Domain.Vendor
  HotJoes.Infrastructure.Persistence

tests/
  HotJoes.Domain.Vendor.Tests
  HotJoes.Application.Vendor.Tests
  HotJoes.IntegrationTests
```

Production projects belong under src.

Test projects belong under tests.

### 3.2 Project Dependencies

Dependencies must point inward toward the domain.

The intended dependency direction is:

```csharp
HotJoes.Api
    ↓
HotJoes.Application.Vendor
    ↓
HotJoes.Domain.Vendor
```

Infrastructure may implement interfaces required by the application layer, but the domain must not depend on infrastructure.

The domain layer must not reference:

- ASP.NET Core;
- Entity Framework Core;
- database providers;
- message broker SDKs;
- HTTP clients;
- logging implementations;
- configuration frameworks.

Circular project references are prohibited.

### 3.3 Domain Ownership

Each domain owns its own entities, rules, and persistence model.

The Vendor domain owns concepts such as:

- Vendor;
- VendorId;
- Menu;
- MenuItem;
- vendor onboarding rules;
- vendor menu rules.

Other domains must not directly reuse or manipulate Vendor domain entities.

Where another domain needs to refer to a vendor, it should normally use its own representation of the required information, including a reference to the vendor identifier where appropriate.

A service boundary must not be bypassed by directly reading or updating another domain’s database tables.

## 4. Namespaces and File Organisation

### 4.1 File-Scoped Namespaces

C# files should use file-scoped namespaces.

Preferred:

```csharp
namespace HotJoes.Domain.Vendor;
```

Avoid:

```csharp
namespace HotJoes.Domain.Vendor
{
    public sealed class Vendor
    {
    }
}
```

File-scoped namespaces reduce indentation and make the main contents of the file easier to read.

### 4.2 One Primary Type Per File

Each file should normally contain one primary class, record, interface, enum, or struct.

Small private or tightly coupled nested types may remain in the same file where this improves readability.

The filename must match the primary type name.

Example:

```csharp
Vendor.cs
VendorId.cs
CreateVendorCommand.cs
CreateVendorHandler.cs
```

### 4.3 Namespace Structure

Namespaces should reflect project structure and responsibility.

Examples:

```csharp
HotJoes.Domain.Vendor
HotJoes.Domain.Vendor.Menus
HotJoes.Application.Vendor.Onboarding
HotJoes.Infrastructure.Persistence.Vendor
```

Do not create deeply nested namespaces unless the additional grouping genuinely improves navigation.

## 5. Naming Standards

### 5.1 General Naming

Names must describe purpose rather than implementation detail.

Use:

- nouns for types;
- verbs or verb phrases for methods;
- questions for Boolean values;
- business terminology where the concept belongs to the domain.

Preferred:

```csharp
IsPublished
CanAcceptOrders
CreateMenu
PublishMenu
VendorRegistration
```

Avoid:

```csharp
Flag
ProcessData
HandleThing
DoWork
Manager
Helper
Utility
```

Generic names such as Manager, Processor, Service, Helper, and Handler must not be used unless the name clearly describes the responsibility.

### 5.2 C# Naming Conventions

Use PascalCase for:

- classes;
- records;
- structs;
- interfaces;
- enums;
- public methods;
- properties;
- constants.

Use camelCase for:

- parameters;
- local variables;
- private fields.

Private fields should use an underscore prefix:

```csharp
private readonly IVendorRepository _vendorRepository;
```

Interfaces should use the I prefix:

```csharp
public interface IVendorRepository
```

Asynchronous methods should use the Async suffix:

```csharp
GetVendorAsync
SaveChangesAsync
PublishMenuAsync
```

### 5.3 Boolean Names

Boolean names must describe the true condition.

Preferred:

```csharp
isPublished
hasActiveMenu
canAcceptOrders
```

Avoid:

```csharp
publishedFlag
status
check
value
```

Avoid negative Boolean names where they make conditions difficult to read.

Less clear:

```csharp
if (!vendor.IsNotActive)
```

Clearer:

```csharp
if (vendor.IsActive)
```

## 6. Types and Domain Modelling

### 6.1 Use Strongly Typed Identifiers

Domain identifiers should use explicit types rather than exposing Guid, int, or string throughout the codebase.

Example:

```csharp
public readonly record struct VendorId(Guid Value);
```

Method signatures should then use:

```csharp
VendorId vendorId
```

rather than:

```csharp
Guid vendorId
```

This prevents identifiers for unrelated concepts from being accidentally interchanged.

For example, a method accepting VendorId cannot accidentally be passed a MenuId without an explicit conversion.

Strongly typed identifiers should be introduced where they provide meaningful type safety. They should not become elaborate mini-frameworks.

### 6.2 Explicit Types and var

Use var where the assigned type is immediately obvious from the right-hand side.

Acceptable:

```csharp
var vendor = new Vendor(vendorId, name);
var menuItems = new List<MenuItem>();
```

Use an explicit type where it improves clarity.

Preferred:

```csharp
Vendor? vendor = await repository.FindAsync(vendorId, cancellationToken);
```

Avoid using var when the reader must inspect another method or infer a non-obvious return type.

The rule is readability, not ideological consistency.

### 6.3 Nullability

Nullable reference types must remain enabled.

Null must represent a legitimate absence of value, not incomplete object construction or deferred validation.

Use nullable types explicitly:

```csharp
string? description
Vendor? vendor
```

Do not suppress nullable warnings without understanding and documenting the reason.

The null-forgiving operator should be rare:

```csharp
value!
```

It must not be used merely to silence compiler warnings.

### 6.4 Records, Classes, and Structs

Use classes for entities with identity and lifecycle.

Use records or readonly record structs for immutable values where value-based equality is appropriate.

Typical examples:

```csharp
public sealed class Vendor
public sealed class Menu
public sealed record VendorName
public readonly record struct VendorId
```

Do not use records automatically for all data-carrying types. Consider identity, mutability, equality, and serialisation behaviour.

### 6.5 Collections

Expose collections in a way that prevents uncontrolled modification.

Preferred:

```csharp
private readonly List<MenuItem> _items = [];

public IReadOnlyCollection<MenuItem> Items => _items;
```

Avoid exposing mutable collections directly:

```csharp
public List<MenuItem> Items { get; set; }
```

Collection changes that affect domain validity must occur through explicit methods.

## 7. Classes and Methods

### 7.1 Single Responsibility

A class should have one coherent responsibility.

This does not mean every class must contain only one method. It means the methods should contribute to the same purpose.

Warning signs include:

- unrelated dependencies;
- unrelated reasons for change;
- names such as VendorManager;
- methods covering validation, persistence, mapping, messaging, and formatting in one class;
- large numbers of private methods representing separate workflows.

### 7.2 Method Size

Methods should be small enough that their intent and control flow can be understood without excessive scrolling.

There is no absolute line-count rule.

A method should be split when it:

- performs multiple distinct operations;
- mixes business rules and infrastructure details;
- contains deeply nested conditions;
- requires comments to divide it into separate phases;
- cannot be named accurately with one clear verb phrase.

Do not split code into trivial methods merely to achieve a line-count target.

### 7.3 Parameters

Methods should accept only the information they need.

Avoid long parameter lists.

Where several values form one meaningful concept, consider introducing a command, request, or value object.

Preferred:

```csharp
CreateVendor(CreateVendorCommand command)
```

instead of:

```csharp
CreateVendor(
    string name,
    string tradingName,
    string email,
    string telephone,
    string addressLine1,
    string addressLine2,
    string postcode)
```

Do not use a large request object merely as an unstructured bag of parameters. The object should represent a meaningful operation.

### 7.4 Return Values

Methods should return the most precise meaningful type.

Avoid returning object, loosely structured dictionaries, tuples with unclear members, or generic success flags where a richer result is required.

Poor:

```csharp
bool CreateVendor(...)
```

Potentially better:

```csharp
CreateVendorResult CreateVendor(...)
```

Where failure is exceptional, throwing an appropriate exception may be correct.

Where failure is an expected business outcome, return an explicit result or domain outcome.

### 7.5 Static State

Mutable global or static state is prohibited unless there is a specific, justified reason.

Examples of unacceptable global state include:

- static current-user values;
- globally mutable configuration;
- static service locators;
- static database contexts;
- static request data;
- global caches without explicit ownership or lifecycle.

Constants and immutable shared values are acceptable.

## 8. Domain Entities and Behaviour

### 8.1 Constructors

Constructors must establish a valid initial state.

Required values should be supplied during construction.

Poor:

```csharp
var vendor = new Vendor();
vendor.Name = name;
vendor.Email = email;
vendor.Status = VendorStatus.Active;
```

Preferred:

```csharp
var vendor = Vendor.Register(vendorId, name, email);
```

or:

```csharp
var vendor = new Vendor(vendorId, name, email);
```

Do not create temporarily invalid domain objects and rely on later property assignments to complete them.

### 8.2 Property Setters

Do not trigger significant domain behaviour from property setters.

Unacceptable examples include setters that:

- publish events;
- save data;
- call external services;
- change other entities;
- alter several unrelated properties;
- silently update status;
- perform complex validation;
- calculate financial values with side effects.

Poor:

```csharp
public bool IsPublished
{
    get => _isPublished;
    set
    {
        _isPublished = value;
        PublishedAt = DateTime.UtcNow;
        DomainEvents.Add(new MenuPublishedEvent(Id));
    }
}
```

Preferred:

```csharp
public void Publish(DateTimeOffset publishedAt)
{
    if (_items.Count == 0)
    {
        throw new MenuCannotBePublishedException(Id);
    }

    IsPublished = true;
    PublishedAt = publishedAt;

    AddDomainEvent(new MenuPublishedEvent(Id));
}
```

Simple validation or normalisation inside a setter may be acceptable, but explicit methods are preferred for meaningful state changes.

### 8.3 State Changes

Important state changes must occur through named methods.

Examples:

```csharp
vendor.Activate()
vendor.Suspend(reason)
menu.AddItem(item)
menu.Publish(publishedAt)
menu.Withdraw(withdrawnAt)
```

The method name must communicate the business operation.

Avoid unrestricted public setters on domain entities.

### 8.4 Domain Events

Domain events should represent something that has already happened.

Examples:

```csharp
VendorRegistered
MenuPublished
VendorSuspended
```

Avoid vague event names:

```csharp
VendorChanged
DataUpdated
ProcessCompleted
```

Creating a domain event must not itself hide infrastructure behaviour inside the entity.

The entity records the event. Application or infrastructure code is responsible for dispatching it.

Event dispatch and persistence consistency must be explicitly designed and tested.

## 9. Application Layer

### 9.1 Application Responsibilities

The application layer coordinates a use case.

It may:

- load domain objects;
- call domain behaviour;
- invoke repositories;
- enforce application-level permissions;
- coordinate transactions;
- publish integration messages through an abstraction;
- return application results.

It should not contain domain rules that belong inside domain objects or domain services.

### 9.2 Commands and Queries

Commands change system state.

Queries retrieve data without changing state.

Examples:

```csharp
CreateVendorCommand
AddMenuItemCommand
PublishMenuCommand

GetVendorQuery
GetVendorMenuQuery
```

A command handler should not become a general service class containing unrelated operations.

A query must not have hidden write behaviour.

Strict CQRS infrastructure is not required merely to follow this separation.

### 9.3 Application Handlers

Handlers should expose the complete use-case flow clearly.

A typical handler may:

1. validate the request structure;
1. load required data;
1. check whether the operation can proceed;
1. invoke domain behaviour;
1. persist changes;
1. return an outcome.

The handler must not conceal important flow through extensive reflection, attributes, or implicit pipelines.

Cross-cutting behaviour may use middleware or pipeline mechanisms where this genuinely improves consistency, but the behaviour and ordering must be explicit and documented.

### 9.4 Mapping

Mapping between API contracts, application models, domain objects, and persistence models must be explicit.

Manual mapping is the default.

A mapping library may be introduced when it demonstrably reduces repetitive mapping without obscuring:

- renamed fields;
- calculated values;
- ignored fields;
- null handling;
- business transformations;
- object ownership.

Do not automatically map API input directly into domain entities.

## 10. Persistence and Entity Framework Core

### 10.1 Persistence Is Infrastructure

Entity Framework types and configuration belong in the infrastructure layer.

Domain entities must not require EF Core references.

Persistence concerns must not dictate public domain behaviour.

### 10.2 Explicit Entity Configuration

Entity relationships, keys, indexes, conversions, constraints, and delete behaviour must be configured explicitly.

Use dedicated configuration classes where appropriate:

```csharp
internal sealed class VendorConfiguration
    : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.HasKey(vendor => vendor.Id);

        builder.Property(vendor => vendor.Id)
            .HasConversion(
                id => id.Value,
                value => new VendorId(value));

        builder.Property(vendor => vendor.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
```

Do not depend on naming conventions for important database behaviour merely because EF can infer it.

### 10.3 Relationships and Cascades

Relationships and delete behaviour must be deliberately configured by the developer and enforced by the database.

Cascade deletion is neither automatically good nor automatically bad.

For every relationship, decide explicitly whether deletion should:

- cascade;
- be restricted;
- set a foreign key to null;
- require explicit dependent deletion;
- be prohibited by business rules.

Example:

```csharp
builder.HasMany(vendor => vendor.Menus)
    .WithOne()
    .HasForeignKey(menu => menu.VendorId)
    .OnDelete(DeleteBehavior.Restrict);
```

The chosen behaviour must reflect domain ownership and data-integrity requirements.

Do not rely on EF defaults for significant delete behaviour.

### 10.4 Repositories

Repositories should represent domain persistence needs, not provide generic table access.

Preferred:

```csharp
public interface IVendorRepository
{
    Task<Vendor?> FindAsync(
        VendorId vendorId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Vendor vendor,
        CancellationToken cancellationToken);
}
```

Avoid generic repositories such as:

```csharp
IRepository<TEntity>
```

unless a concrete and justified requirement emerges.

A repository should normally work with an aggregate root rather than exposing unrestricted CRUD access to every persistence entity.

### 10.5 Database Queries

Queries must be written with awareness of their generated database behaviour.

Developers must consider:

- number of database round trips;
- loading unnecessary columns or relationships;
- unbounded result sets;
- missing indexes;
- client-side evaluation;
- N+1 query behaviour;
- tracking where it is not required.

Read-only queries should normally use no-tracking behaviour.

Pagination must be used where a query could return a large result set.

### 10.6 Transactions

Transaction boundaries must align with application use cases.

Do not create one broad transaction that spans unrelated operations or unreliable external calls.

Database changes and integration-message publication require an explicit consistency strategy.

Where reliable message publication is required, an outbox pattern should be considered rather than pretending that a database transaction and message broker operation form one atomic transaction.

### 10.7 Migrations

Database schema changes must be made through reviewed migrations.

Migrations must be:

- named clearly;
- committed to source control;
- reviewed for destructive operations;
- tested against the supported database;
- applied through a controlled deployment process.

Production schema changes must not depend on automatic database creation at application startup.

## 11. Asynchronous Code

### 11.1 Use Async for Genuine Asynchronous Work

Asynchronous code should be used for I/O-bound work such as:

- database calls;
- HTTP calls;
- message broker operations;
- file operations;
- other external service calls.

Do not make a method asynchronous merely because asynchronous programming is fashionable or theoretically scalable.

Do not wrap synchronous CPU work in Task.Run inside normal server application code unless there is a specific and justified reason.

### 11.2 Async Naming

Methods returning Task or ValueTask should use the Async suffix.

Example:

```csharp
FindVendorAsync
SaveChangesAsync
PublishMessageAsync
```

### 11.3 Cancellation Tokens

A cancellation token allows the caller to signal that an operation is no longer required.

For example, an HTTP request may be abandoned because:

- the client disconnected;
- a request timeout was reached;
- the application is shutting down;
- a parent operation was cancelled.

Public asynchronous application and infrastructure methods that perform cancellable I/O should normally accept a CancellationToken.

Example:

```csharp
public async Task<Vendor?> FindAsync(
    VendorId vendorId,
    CancellationToken cancellationToken)
{
    return await _dbContext.Vendors
        .SingleOrDefaultAsync(
            vendor => vendor.Id == vendorId,
            cancellationToken);
}
```

Tokens should be passed through the call chain to underlying operations that support cancellation.

Do not introduce a cancellation token into purely synchronous domain methods.

Cancellation does not automatically roll back completed work. State integrity must still be protected through transaction boundaries and operation design.

Do not blindly cancel an operation after it has passed a point where abandoning it would leave inconsistent state.

### 11.4 Avoid Sync-Over-Async

Do not use:

```csharp
.Result
.Wait()
.GetAwaiter().GetResult()
```

in normal asynchronous application flow.

These patterns can block threads and create deadlock or throughput problems.

### 11.5 ValueTask

Use Task by default.

Use ValueTask only after measuring a genuine performance benefit and understanding its usage restrictions.

## 12. Error and Exception Handling

### 12.1 Expected Outcomes Versus Exceptions

Expected business outcomes should not automatically be represented as exceptions.

Examples of expected outcomes may include:

- vendor already exists;
- menu cannot yet be published;
- requested vendor was not found;
- item is no longer available;
- registration details failed validation.

These may be represented by application result types, domain results, or appropriate API responses.

Exceptions should represent situations where the normal operation cannot continue and the condition is exceptional at that layer.

### 12.2 Domain Exceptions

A domain exception may be appropriate where a domain invariant is violated and the calling code is not expected to proceed.

Examples:

```csharp
MenuCannotBePublishedException
VendorAlreadySuspendedException
InvalidVendorStateException
```

Exceptions must be specific enough to communicate the failure.

Avoid throwing:

```csharp
Exception
ApplicationException
InvalidOperationException
```

for known domain conditions when a more meaningful type should exist.

Do not create a separate exception type for every trivial validation failure.

### 12.3 Catching Exceptions

Catch an exception only when the code can do something meaningful, such as:

- translate it into a layer-appropriate outcome;
- add useful context;
- perform a controlled retry;
- compensate for partial work;
- log it at the correct system boundary;
- return an appropriate API response.

Do not catch an exception merely to rethrow the same exception.

Poor:

```csharp
try
{
    await operation.ExecuteAsync();
}
catch (Exception exception)
{
    throw exception;
}
```

This also damages the original stack trace.

If an exception must be rethrown unchanged:

```csharp
throw;
```

### 12.4 API Error Responses

API responses must not expose:

- stack traces;
- internal exception messages;
- database details;
- SQL;
- server paths;
- internal class names;
- connection information;
- framework diagnostics.

The API should return a controlled error contract containing only information safe and useful to the client.

A typical response may include:

- an error code;
- a client-safe message;
- validation details;
- a correlation identifier.

Internal diagnostic information belongs in logs, not in the client response.

### 12.5 Global Exception Handling

Unexpected API exceptions should be handled at a central boundary.

The global handler should:

- map known exception types to controlled HTTP responses;
- log unexpected failures;
- preserve correlation information;
- avoid leaking internal details;
- avoid duplicating logs already written at a more appropriate boundary.

Central handling must not turn every failure into HTTP 500.

## 13. Validation

### 13.1 Validation Responsibilities

Validation exists at several levels.

API validation

Checks whether the incoming request is structurally usable.

Examples:

- required JSON field missing;
- malformed email address;
- string exceeds contract length;
- invalid enum value.

Application validation

Checks whether the requested use case can be attempted.

Examples:

- caller lacks permission;
- referenced vendor does not exist;
- duplicate registration request;
- request conflicts with current application state.

Domain validation

Protects domain invariants.

Examples:

- a menu cannot be published without items;
- a suspended vendor cannot publish a menu;
- a price cannot be negative.

Validation must occur at the layer that owns the rule.

### 13.2 Duplicate Validation

Some validation may deliberately exist at more than one boundary.

For example:

- the API can reject an empty name quickly;
- the domain must still prevent an entity being created with an empty name.

The domain must not rely on the API having validated correctly.

## 14. Dependency Injection and Configuration

### 14.1 Constructor Injection

Dependencies should normally be supplied through constructor injection.

Preferred:

```csharp
public sealed class CreateVendorHandler
{
    private readonly IVendorRepository _vendorRepository;

    public CreateVendorHandler(
        IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }
}
```

Avoid:

- service locator patterns;
- resolving services manually throughout application code;
- static dependency access;
- optional hidden dependencies.

### 14.2 Dependency Lifetimes

Service lifetimes must be selected deliberately.

Particular care is required when:

- injecting scoped services into singleton services;
- retaining request-specific state;
- using database contexts;
- maintaining caches;
- registering message consumers.

The container accepting a registration does not prove the lifetime is correct.

### 14.3 Configuration

Configuration should be:

- strongly typed where practical;
- validated at application startup;
- grouped by responsibility;
- supplied through supported configuration mechanisms;
- free of secrets committed to source control.

Avoid arbitrary calls to configuration by string key throughout the codebase.

Preferred:

```csharp
public sealed class MessageBrokerOptions
{
    public required string Namespace { get; init; }
    public required string TopicName { get; init; }
}
```

Configuration values are infrastructure inputs, not mutable global state.

## 15. Logging and Observability

### 15.1 Structured Logging

Use structured logging.

Preferred:

```csharp
_logger.LogInformation(
    "Vendor {VendorId} registered successfully",
    vendor.Id);
```

Avoid:

```csharp
_logger.LogInformation(
    $"Vendor {vendor.Id} registered successfully");
```

Structured properties support filtering, searching, and correlation.

### 15.2 Logging Levels

Use logging levels consistently.

- Trace: highly detailed diagnostic flow.
- Debug: development and detailed troubleshooting information.
- Information: meaningful normal system events.
- Warning: unexpected or degraded behaviour from which the system recovered.
- Error: an operation failed.
- Critical: a severe failure affecting continued system operation.

Do not log normal validation failures as system errors.

### 15.3 Sensitive Information

Do not log:

- passwords;
- authentication tokens;
- full payment details;
- private keys;
- connection strings;
- unnecessary personal data;
- full request bodies by default.

Logging must be designed with the assumption that operational staff and external systems may access the logs.

### 15.4 Correlation

Requests and messages should carry correlation information where practical.

Logs related to one business operation should be traceable across:

- API handling;
- application execution;
- database activity;
- message publication;
- message consumption.

Correlation does not replace meaningful log messages.

## 16. API Standards

### 16.1 API Contracts

API request and response models must be separate from domain entities.

Do not expose EF entities or domain aggregates directly through controllers.

Contracts must represent the API boundary, not the internal object model.

### 16.2 Controllers and Endpoints

Controllers or endpoint handlers should remain thin.

They may:

- bind and validate requests;
- invoke the application layer;
- translate application outcomes into HTTP responses;
- pass cancellation tokens;
- add boundary-specific metadata.

They should not contain:

- domain rules;
- database queries;
- persistence transactions;
- entity construction logic;
- direct message-broker calls.

### 16.3 HTTP Status Codes

Use status codes according to the actual result.

Examples:

- 200 OK for a successful retrieval or update with a response body;
- 201 Created for successful resource creation;
- 204 No Content for successful completion without a response body;
- 400 Bad Request for malformed or structurally invalid input;
- 401 Unauthorized where authentication is required;
- 403 Forbidden where the caller is authenticated but not permitted;
- 404 Not Found where the requested resource does not exist;
- 409 Conflict where the request conflicts with current resource state;
- 500 Internal Server Error for unexpected server failures.

Do not return 200 OK with a body stating that the operation failed.

### 16.4 Versioning and Compatibility

Breaking changes to published API contracts must be deliberate.

Do not silently rename, remove, or change the meaning of public fields.

API versioning should be introduced when an actual compatibility requirement emerges. It should not be added as ceremonial infrastructure before it is needed.

## 17. Messaging Standards

### 17.1 Message Ownership

The publishing domain owns the meaning of an integration event.

Consumers must not depend on the publisher’s internal domain classes.

Integration contracts should be explicit and versioned where required.

### 17.2 Event Names

Integration-event names should describe completed facts.

Examples:

```csharp
VendorRegistered
VendorActivated
MenuPublished
```

Avoid command-like event names:

```csharp
RegisterVendor
PublishMenu
```

A command requests that something happen.

An event states that something has happened.

### 17.3 Message Content

Messages should contain the information required by consumers without exposing the publisher’s entire internal object model.

Do not place full domain entities into message payloads.

Avoid forcing every consumer to call back into the publisher immediately merely because the event omitted essential information.

The appropriate balance must be decided per event.

### 17.4 Idempotency

Message consumers must assume that the same message may be delivered more than once.

Where duplicate processing could cause incorrect behaviour, consumers must implement idempotency.

Examples include:

- storing processed message identifiers;
- enforcing database uniqueness;
- checking current state before applying a transition.

“Exactly once” must not be assumed merely because a broker offers delivery guarantees.

### 17.5 Failure Handling

Message-processing failures must have an explicit strategy.

This may include:

- bounded retries;
- dead-letter queues;
- poison-message handling;
- operational alerts;
- replay procedures.

Infinite retries are prohibited.

Retrying an operation that is not safe to repeat is also prohibited.

## 18. Testing-Related Coding Standards

Detailed testing policy belongs in the HotJoes Testing Strategy and Standards document. The following rules apply directly to production-code design.

### 18.1 Testable Design

Code should be testable through normal public behaviour.

Do not expose private implementation details solely to make tests possible.

Do not make every method virtual merely for mocking.

Prefer testing meaningful outcomes rather than reproducing implementation steps.

### 18.2 Deterministic Behaviour

Tests and production code should avoid uncontrolled dependencies on:

- current system time;
- random values;
- static state;
- environment-specific configuration;
- network services;
- execution order.

Where time affects behaviour, inject or pass an explicit time source.

Poor:

```csharp
PublishedAt = DateTimeOffset.UtcNow;
```

Preferred where deterministic control is required:

```csharp
menu.Publish(clock.UtcNow);
```

Do not introduce abstractions such as clocks unless the code genuinely depends on time-sensitive behaviour.

### 18.3 Test Naming

Test names should describe:

- the behaviour under test;
- the relevant condition;
- the expected result.

Example:

```csharp
Publish_WhenMenuContainsNoItems_ThrowsMenuCannotBePublishedException
```

or:

```csharp
Publish_WithAtLeastOneItem_MarksMenuAsPublished
```

The exact naming format may vary, but the test’s purpose must be immediately clear.

## 19. Code Formatting and Language Usage

### 19.1 Automated Formatting

The repository .editorconfig is the source of truth for supported formatting rules.

All code must pass:

```csharp
dotnet format
```

Formatting should not be debated repeatedly during reviews where an automated rule can settle it.

### 19.2 Braces

Use braces for control-flow statements, including single-line bodies.

Preferred:

```csharp
if (vendor is null)
{
    return VendorResult.NotFound(vendorId);
}
```

Avoid:

```csharp
if (vendor is null)
    return VendorResult.NotFound(vendorId);
```

Braces reduce errors during later modification.

### 19.3 Expression-Bodied Members

Expression-bodied members may be used where they improve readability.

Appropriate:

```csharp
public bool IsActive => Status == VendorStatus.Active;
```

Avoid compressing multi-step logic into dense expressions merely to reduce line count.

### 19.4 Pattern Matching

Use pattern matching where it makes code clearer.

Example:

```csharp
if (vendor is null)
{
    return VendorResult.NotFound(vendorId);
}
```

Do not use complex nested patterns where ordinary control flow would be easier to understand.

### 19.5 LINQ

Use LINQ for clear collection queries and transformations.

Avoid:

- deeply nested LINQ expressions;
- complex business workflows hidden in a chain;
- side effects inside LINQ operations;
- repeated enumeration of expensive queries;
- unreadable one-line expressions.

A straightforward loop is acceptable and often preferable.

### 19.6 Ternary Operators

Use ternary operators only for simple expressions.

Acceptable:

```csharp
var status = vendor.IsActive
    ? VendorStatusDto.Active
    : VendorStatusDto.Inactive;
```

Avoid nested ternary expressions.

### 19.7 Constants and Magic Values

Business-significant values must not be scattered through the code as unexplained literals.

Poor:

```csharp
if (name.Length > 200)
```

Preferred:

```csharp
public const int MaximumNameLength = 200;
```

The constant should live with the concept that owns it.

Do not create constants for obvious one-off implementation values where naming them adds no clarity.

## 20. Security Standards

### 20.1 Treat All External Input as Untrusted

External input includes:

- API requests;
- message payloads;
- files;
- environment variables;
- database data originating from external systems;
- identity claims;
- HTTP responses.

Input must be validated and safely handled at the appropriate boundary.

### 20.2 Authorisation

Authentication proves identity.

Authorisation determines whether that identity may perform an operation.

Authorisation must be enforced at the application or API boundary before protected state is changed.

Do not rely solely on the user interface hiding an action.

### 20.3 Secrets

Secrets must not be:

- committed to Git;
- stored in source files;
- placed in normal configuration files;
- logged;
- returned through API responses.

Use environment-specific secret-management mechanisms.

### 20.4 Database Safety

Use parameterised queries or supported ORM query mechanisms.

Do not construct SQL by concatenating external input.

Any use of raw SQL must be justified, reviewed, parameterised, and tested.

## 21. Performance Standards

### 21.1 Measure Before Optimising

Code should be reasonably efficient, but speculative optimisation is discouraged.

Optimisation must be driven by:

- measured performance;
- known scale requirements;
- identified bottlenecks;
- infrastructure constraints.

Do not sacrifice clarity for theoretical micro-optimisation.

### 21.2 Avoid Known Inefficiencies

Developers are still expected to avoid obvious problems such as:

- repeated database calls in loops;
- loading complete tables into memory;
- unbounded API responses;
- unnecessary object allocation in hot paths;
- serial external calls that could safely run concurrently;
- parallel operations that threaten data integrity;
- unnecessary network round trips.

Concurrency must not be introduced where ordering or state integrity requires sequential execution.

## 22. Review Standards

A code review should determine whether the change:

- implements the intended behaviour;
- preserves domain boundaries;
- makes important behaviour explicit;
- introduces unnecessary abstraction;
- handles failure correctly;
- protects data integrity;
- includes appropriate tests;
- follows naming and formatting standards;
- introduces security or operational risks;
- adds hidden framework behaviour;
- duplicates existing functionality;
- leaves obsolete code behind.

Review comments should identify the concrete risk or standard involved.

“Best practice” is not, by itself, an adequate review justification.

A reviewer should explain why a proposed change improves this system.

## 23. Prohibited Practices

The following practices are prohibited unless a documented exception is approved:

- mutable global state;
- service locator usage;
- generic repositories introduced without a concrete need;
- domain entities exposed directly through API contracts;
- unrestricted public setters on domain entities;
- significant behaviour in property setters;
- swallowing exceptions;
- returning internal exception details to clients;
- empty catch blocks;
- synchronous blocking of asynchronous operations;
- implicit destructive cascade behaviour;
- database access from controllers;
- business rules in controllers;
- direct access to another domain’s database;
- committed secrets;
- infinite retries;
- logging sensitive data;
- tests that depend on execution order;
- reflection-heavy conventions that obscure control flow;
- abstractions created only for hypothetical future use.

## 24. Exceptions to These Standards

A standard may be broken where following it would clearly produce a worse technical result.

The exception must be deliberate rather than accidental.

The developer should document, in the code review or relevant design record:

- which standard is being departed from;
- why the normal rule is unsuitable;
- what risks the exception introduces;
- how those risks are controlled.

“Because the framework generated it” is not sufficient justification.

“Because this is how we have always done it” is not sufficient justification.

## 25. Enforcement

These standards will be enforced through a combination of:

- .editorconfig;
- compiler warnings;
- static analysis;
- dotnet format;
- automated build checks;
- automated tests;
- dependency validation where appropriate;
- pull-request review;
- architectural tests where they provide genuine value.

Rules should be automated where enforcement is objective and reliable.

Rules requiring engineering judgement must remain review responsibilities rather than being forced through brittle tooling.

The separate HotJoes Enforcement Strategy defines how these controls are introduced and maintained.

## 26. Definition of Compliant Code

Code is compliant when it:

- is understandable to another competent developer;
- expresses business behaviour explicitly;
- respects domain and project boundaries;
- maintains valid state;
- handles expected and unexpected failures appropriately;
- does not depend on unexplained framework behaviour;
- follows the automated repository rules;
- includes the level of testing required by the Testing Strategy;
- can be operated and diagnosed safely;
- introduces no unjustified abstraction or complexity.

Compliance is not achieved merely because the code compiles, passes tests, or satisfies a static analyser.

The code must also be structurally sound and understandable.

## 27. Initial Tooling Implications

The first enforcement pass should include rules for:

- nullable reference types;
- consistent namespace style;
- file formatting;
- unused imports;
- avoidable compiler warnings;
- naming conventions where reliable;
- asynchronous method naming;
- final newlines;
- line endings;
- trailing whitespace;
- build failure on selected warnings.

More subjective standards should initially be enforced through review rather than by enabling a large analyser ruleset without examining each rule.

No analyser package should be introduced merely to create the appearance of rigour.

Every enabled rule must have a known purpose and an agreed severity.

## 28. Document Maintenance

This document is expected to evolve as the HotJoes system gains real implementation experience.

Changes should be made when:

- a recurring code-review issue emerges;
- a framework or tool is adopted;
- an existing rule proves unclear or counterproductive;
- a new architectural pattern is introduced;
- an enforcement rule is added;
- production experience reveals an operational risk.

Standards should not be changed merely to justify code that was written without following them.

**End of document**

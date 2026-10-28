# HotJoes

HotJoes is a cloud-native, event-driven marketplace being designed for independent food vendors, delivery drivers and customers. Its commercial aim is to support a fairer and more transparent food-ordering ecosystem; its engineering aim is to provide a serious reference implementation of modern application architecture, disciplined delivery and accountable AI-assisted software development.

The platform is being delivered incrementally. The current implementation concentrates on the first end-to-end capability: Vendor Registration, pre-registration Compliance determination, reliable event publication, post-registration Community participation and retrieval of the authoritative registered Vendor.

This repository is therefore more than a code sample. It records the reasoning that shaped the system, the decisions that constrain it, the contracts that connect it, the tests that enforce it and the implementation that proves those decisions can work together.

## A guided route through the repository

## Demonstration

Watch or download the [HotJoes Epic 1 Vendor Registration demonstration](https://github.com/Joasties/HotJoes/releases/download/epic1-demonstration-v1/Demonstration.mov).

The demonstration exercises the rendered Vendor Registration journey through the disposable local runtime, including authoritative registration, retrieval, Compliance processing and optional Community participation.

HotJoes contains several layers of information because each layer answers a different question. The quickest route from the original idea to the executable system is:

```text
Vision and visual models
          ↓
Architectural concerns and patterns
          ↓
Approved decisions and architecture
          ↓
Service contracts and test catalogues
          ↓
Executable tests
          ↓
Source code and running demonstration
```

### 1. Begin with the platform vision

Read the [Project Vision](<docs/architecture/HJ-001 - Project Vision.md>) first. It explains the commercial idea, the participants the platform intends to serve, its fair-marketplace principles and the incremental roadmap from Vendor onboarding to ordering, payments and delivery.

This is the stable answer to **why HotJoes exists**. It deliberately avoids making detailed implementation decisions.

### 2. Use the Epic 1 visual models for orientation

The [`docs/images/Epic1`](docs/images/Epic1) folder provides a visual introduction to the first delivery increment:

- [Epic 1 System Model](<docs/images/Epic1/HJ-SM-001 - System Model.png>) shows the initial system landscape, business capabilities and collaboration paths;
- [Vendor Domain conceptual model](<docs/images/Epic1/Vendor Domain Model Conceptual.png>) provides an accessible starting view of the Vendor model; and
- the [Vendor Registration screen sequence](<docs/images/Epic1/HotJoes Step 1.png>) introduces the intended user journey, with the remaining steps available in the same folder.

These models and mock-ups are orientation and design evidence. They make the system easier to approach, but they do not override later approved architecture, contracts or field definitions. Where an artefact differs from the current approved baseline, the baseline wins.

### 3. Follow the concern roadmap

The [Architecture Concern Working Register](<docs/tools/HJ Architecture Concern Working Register.xlsx>) is the working roadmap that was used to explore, resolve and track the architectural concerns and patterns required for Epic 1.

It records the route through the problem: what needed to be decided, why it mattered, which concerns depended on others and when each concern became sufficiently resolved to implement. It is a working and historical navigation aid rather than a replacement for the approved architecture documents.

### 4. Inspect how each decision became controlled architecture

Four connected collections preserve the decision trail:

- [`docs/adr`](docs/adr) records the significant architectural decisions and their consequences;
- [`docs/architecture`](docs/architecture) contains the authoritative architectural layers, including vision, principles, language, domain models, standards, scope, patterns, contracts and test catalogues;
- [`docs/ai/prompts`](docs/ai/prompts) defines the controlled procedures used to analyse, propagate, infer, review and implement architectural work with AI assistance; and
- [`docs/cr`](docs/cr) preserves the Change Requests through which each approved change was reconciled into the controlled baseline.

Together they answer **what was decided, why it was decided, which artefacts were affected and how consistency was restored**.

The [Current Approved Baseline Manifest](<docs/architecture/HJ-000 - Current Approved Baseline Manifest.md>) is the authoritative entry point when exact version and approval status matter.

### 5. Trace architecture into contracts and verification

The approved architecture is translated into explicit service and test artefacts rather than left as prose:

- [Vendor Registration Service Contract](<docs/architecture/HJ-106 - Vendor Registration Service Contract.md>)
- [Vendor Registration Test Catalogue](<docs/architecture/HJ-107 - Vendor Registration Test Catalogue.md>)
- [Architecture and Implementation Test Catalogue](<docs/architecture/HJ-013 - Architecture and Implementation Test Catalogue.md>)
- [`tests`](tests), containing the executable Domain, Application, API, architecture, integration, Web Edge and browser evidence

The identifiers carried into tests make the connection between an architectural obligation and its executable enforcement visible.

### 6. Finish with the implementation

The [`src`](src) folder contains the Domain, Application, Infrastructure, API, Web Edge, workers and Angular client that implement the approved Epic 1 slice. [`compose.yaml`](compose.yaml) assembles those responsibilities into a disposable local runtime using PostgreSQL and RabbitMQ.

Reading the repository in this order makes the source code the conclusion of the design story rather than its unexplained starting point.

## What the current increment demonstrates

The implemented journey allows a prospective Vendor to:

- choose its trading location and legal operator type;
- resolve and select an authoritative business address;
- enter its primary contact and business identity;
- describe weekly opening hours and trading characteristics;
- obtain a deterministic Compliance determination for the required licence types;
- review and submit a complete registration;
- retrieve the persisted registered information; and
- optionally join the HotJoes Community using a preferred future contact channel.

Behind that journey, the implementation demonstrates:

- Domain-Driven Design with explicit bounded-context ownership;
- a rich Vendor Domain separated from Application and Infrastructure concerns;
- ports and adapters around Address, Compliance, persistence and messaging boundaries;
- thin HTTP endpoints and an allowlisted Web Edge;
- PostgreSQL-backed authoritative state and concurrency control;
- idempotent operations and controlled conflict handling;
- transactional outbox staging and immutable integration events;
- RabbitMQ publication, independently hosted consumers, durable receipts and duplicate detection;
- bounded retry, recovery and dead-letter behaviour;
- a feature-oriented Angular client with typed session state and accessible interaction;
- disposable Docker Compose runtime environments; and
- automated architecture, domain, application, API, integration, Web Edge, Angular and full-stack browser verification.

The Address and Compliance integrations deliberately use deterministic Epic 1 adapters behind approved boundaries. They prove the ownership and collaboration patterns without pretending that future external services already exist.

## Architecture

HotJoes is organised around business capabilities rather than technical tiers alone.

The implemented bounded contexts are:

- **Vendor** — authoritative registration, registered information and Vendor identity;
- **Address** — authoritative address search and resolution consumed by registration;
- **Compliance** — determination of the licence types required by a proposed trading model; and
- **Community** — an explicit, post-registration participation record and contact-channel preference.

Each capability owns its behaviour and persistence logically, even where the current local runtime shares physical infrastructure. Collaboration occurs through explicit Application ports, HTTP contracts and versioned Integration Events rather than shared tables or leaked implementation types.

The governing philosophy is **Reliability over Convenience**: an operation should complete authoritatively or fail in a controlled, observable and recoverable way. That principle is visible in transaction boundaries, replay behaviour, concurrency handling, immutable event staging, consumer idempotency and testable failure paths.

Useful starting points include:

- [Project Vision](<docs/architecture/HJ-001 - Project Vision.md>)
- [Architectural Principles](<docs/architecture/HJ-002 - Architectural Principles.md>)
- [Ubiquitous Language](<docs/architecture/HJ-003 - Ubiquitous Language Guide.md>)
- [Established Architecture Patterns](<docs/architecture/HJ-012 - Established Application Architecture Patterns.md>)
- [Architectural Decision Register](<docs/adr/ADR-000 - Architectural Decision Register.md>)
- [Current Approved Baseline](<docs/architecture/HJ-000 - Current Approved Baseline Manifest.md>)

## Human-led, AI-assisted engineering

HotJoes is also an experiment in using AI as a governed engineering collaborator rather than an autonomous code generator.

I retain ownership of the product intent, architectural decisions, acceptance criteria and repository. AI assists with analysis, challenge, consistency checking, option generation, document propagation, contract inference, test design and implementation proposals. It cannot approve its own output or directly change the controlled repository under the normal operating model.

The collaboration typically follows this sequence:

1. **Explore the concern** — identify ambiguity, competing interpretations, ownership boundaries and missing decisions.
2. **Enter decision mode** — develop precise normative statements together until I explicitly approve the outcome.
3. **Propagate the decision** — reconcile the approved change through the authoritative architecture, ubiquitous language, domain model, ADRs and baseline manifest.
4. **Infer contracts and tests** — regenerate service contracts and behavioural and architectural test catalogues from the approved source material rather than inventing them in code.
5. **Slice the implementation** — divide the concern into small, ordered delivery slices with explicit scope and applicable obligations.
6. **Propose tests first** — produce complete test candidates and confirm the expected red state.
7. **Propose production code** — generate complete replacement candidates outside the repository for human inspection and application.
8. **Verify and correct** — build the whole solution, run layered tests, inspect failures and distinguish product defects from test defects.
9. **Review and check in** — retain human approval, protected pull requests and continuous-integration gates as the final authority.

This creates a deliberate chain from business language to architecture, from architecture to contracts, from contracts to tests and from tests to code. When implementation feedback exposes a weak decision, the process moves back up that chain and corrects the source rather than silently normalising divergence.

The approach is intentionally conversational. AI is valuable because it can question assumptions, maintain context across many related artefacts and rapidly generate alternatives. Human judgement remains essential because architectural fitness, business meaning, risk acceptance and final approval are responsibilities, not text-generation tasks.

The governing material is available in the repository:

- [AI Roles and Responsibilities](<docs/architecture/HJ-008 - AI Roles and Responsibilities.md>)
- [AI Operating Guide](<docs/architecture/HJ-009 - AI Operating Guide.md>)
- [Architectural Development Mode](<docs/ai/prompts/PR-000 - Architectural Development Mode.md>)
- [Implement an Approved Architectural Concern](<docs/ai/prompts/PR-006 - Implement Approved Architectural Concern.md>)
- [Global Output and Verification Rules](<docs/ai/prompts/PR-008 - Global Output and Verification Rules.md>)
- [Change Request history](docs/cr)

## Evidence rather than assertion

The documentation is executable wherever practical.

Architecture tests inspect the solution graph, project references, runtime composition, CI workflow, approved obligation catalogue and browser-test governance. Behavioural tests cover Domain and Application rules. Integration tests exercise PostgreSQL, RabbitMQ, outbox relays, hosted workers, API composition and recovery behaviour. Angular tests cover feature state and interaction. Playwright exercises the rendered application through a disposable full-stack environment and retains governed accessibility, diagnostic and visual evidence.

At the current checkpoint, the repository reports:

- **930** successful .NET tests;
- **81** successful Angular tests; and
- **35** successful Playwright tests, including the governed visual baseline.

These counts are useful evidence of breadth, but they are not treated as proof of correctness by themselves. The project also uses manual UX and accessibility review, explicit evidence limitations, protected pull requests and required GitHub Actions quality gates.

## Technology

- .NET 10 and C#
- ASP.NET Core Minimal APIs and YARP
- Angular 22 and TypeScript
- PostgreSQL 18
- RabbitMQ 4
- Docker and Docker Compose
- xUnit, Vitest and Playwright
- GitHub Actions
- Azure as the approved reference production-cloud direction

The current runtime is deliberately local and disposable. The repository does not claim that production identity, authorisation, privacy, operational scale or cloud deployment work is already complete.

## Repository guide

```text
docs/architecture/   Approved architecture and engineering baseline
docs/adr/            Architectural Decision Records
docs/ai/prompts/     Controlled AI-assisted engineering procedures
docs/cr/             Traceable Change Requests
docs/review/         Structured human review protocols and evidence
src/                 Application source and executable hosts
tests/               Layered automated and full-stack browser tests
compose.yaml         Disposable local runtime composition
```

For the current implemented scope, continue with:

- [Epic 1 Implementation Scope](<docs/architecture/HJ-011 - Epic 1 Vendor Registration Implementation Scope.md>)
- [Vendor Registration Service Contract](<docs/architecture/HJ-106 - Vendor Registration Service Contract.md>)
- [Vendor Registration Test Catalogue](<docs/architecture/HJ-107 - Vendor Registration Test Catalogue.md>)
- [Architecture and Implementation Test Catalogue](<docs/architecture/HJ-013 - Architecture and Implementation Test Catalogue.md>)

## Project status

HotJoes is an evolving portfolio and reference project, not a live commercial marketplace. Vendor Registration and its supporting Epic 1 capabilities provide the current vertical slice. Customer ordering, payments, delivery, production identity and authorisation, operational cloud deployment and other marketplace capabilities remain future work governed by the same architecture-first process.

The repository is published to demonstrate both the resulting system and the engineering reasoning used to create it.

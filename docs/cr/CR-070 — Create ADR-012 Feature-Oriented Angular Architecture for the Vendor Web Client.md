# CR-070 — Create ADR-012 Feature-Oriented Angular Architecture for the Vendor Web Client

| Field | Value |
|---|---|
| Change Request | CR-TBD |
| Status | Proposed |
| Target artefact | ADR-012 — Feature-Oriented Angular Architecture for the Vendor Web Client |
| Controlled baseline | New ADR; next sequential identifier after ADR-011 |
| Applicable concern | CON-042 — Web UI application architecture |

## Purpose

Create the lasting architectural decision record required by the explicitly approved CON-042 detailed Angular Web UI architecture.

## Approved decision baseline

The architectural decision-maker explicitly approved CON-042 on 7 September 2026. The decision establishes a feature-oriented standalone Angular application, route-level registration journey, strictly typed reactive forms, an encapsulated feature-scoped Angular signal store, typed Web-client ports and HTTP adapters, explicit submission states and separate registration, retrieval and post-registration boundaries.

## Propagation problem

HJ-010 and HJ-012 can record the approved concern through routine PR-007 propagation, but creation of a substantive individual ADR exceeds routine propagation authority and requires its own formal Change Request.

## Authorised target change

Create ADR-012 with context, the approved decision, rationale, alternatives, consequences and verification. Preserve the authority of HJ-104, HJ-106, HJ-107 and the server-side Domain, Application and HTTP boundaries. Retain CON-038 and CON-044 ownership of runtime/deployment and browser-test architecture.

## Non-goals

Do not select detailed visual design, step-level business rules, precise responsive layouts, browser-test fixtures, diagnostic retention, API Gateway product, runtime configuration delivery, same-origin arrangement, deployment topology, production identity or durable Registration Sessions. Do not amend any other ADR.

## Source authority

CON-042 Approved; HJ-010; HJ-011; HJ-012; ADR-010; CON-031; CON-041; CON-045.

## Document-control treatment

Create ADR-012 v1.0 with Status Accepted after approval and human application. Register it separately in ADR-000 through routine index synchronization.

## Verification and completion

ADR-012 must reproduce the approved decision without extension, identify considered alternatives, preserve all deferred authorities and define verifiable architectural consequences. Completion requires human approval and application of this CR and ADR-012.

This Change Request amends only `ADR-012 — Feature-Oriented Angular Architecture for the Vendor Web Client`. It does not amend any other controlled artefact.

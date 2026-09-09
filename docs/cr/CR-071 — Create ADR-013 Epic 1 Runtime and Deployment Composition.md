# CR-071 — Create ADR-013 Epic 1 Runtime and Deployment Composition

| Field | Value |
|---|---|
| Status | Proposed |
| Target artefact | ADR-013 — Epic 1 Runtime and Deployment Composition |
| Controlled baseline | No existing ADR; ADR-013 is the next available identifier |
| Approved concern | CON-038 |

## Purpose

Create ADR-013 to record the lasting Docker Compose, Azure Container Apps, ASP.NET Core YARP, single-origin ingress, explicit migration and health-gated composition decisions approved through CON-038.

## Authorised Change

Create ADR-013 v1.0 with Accepted status. Record its context, decision, rationale, alternatives, consequences and verification requirements without extending the approved CON-038 decision.

## Non-goals

This CR does not amend HJ-010, HJ-011, HJ-012, ADR-000 or any implementation, contract or test catalogue. It does not select production identity, public exposure, production-scale availability or disaster recovery.

## Verification and Completion

Verify fidelity to the approved CON-038 row, provider isolation, YARP thin-boundary consistency, explicit migration/readiness treatment and absence of new business behaviour. Complete when ADR-013 v1.0 is approved and applied by the human.

This Change Request amends only `ADR-013 - Epic 1 Runtime and Deployment Composition.md`. It does not amend any other controlled artefact.

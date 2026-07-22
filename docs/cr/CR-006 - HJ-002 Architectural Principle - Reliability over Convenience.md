# CR-006 – HJ-002 Architectural Principle: Reliability over Convenience

## Change Request

| Field | Value |
|---|---|
| **Change Request ID** | CR-006 |
| **Target Document** | HJ-002 – Architectural Principles |
| **Version Affected** | Current Draft |
| **Priority** | High |
| **Status** | Approved |
| **Requested By** | Project Architecture |

## Summary

Introduce a new top-level architectural principle establishing **Reliability over Convenience** as the primary engineering philosophy for the HotJoes platform.

This principle shall become the first architectural principle within HJ-002 and establish the project’s expectations regarding correctness, recoverability, observability and operational resilience.

## Rationale

The HotJoes platform is intended to be a commercial-grade distributed system. Engineering decisions shall prioritise correctness, reliability and recoverability over implementation convenience or short-term development speed.

The architecture shall minimise opportunities for defects, preserve system integrity during failure conditions and provide sufficient operational visibility to diagnose and recover from unexpected events.

This principle reflects the project’s long-term engineering philosophy and shall influence all subsequent architectural, implementation and operational decisions.

## Required Changes

### 1. Insert New Principle

Insert a new first architectural principle before all existing principles.

### Title

**AP-000 – Reliability over Convenience**

### Principle Statement

The HotJoes platform shall prioritise correctness, reliability and recoverability over convenience of implementation.

Software shall either complete successfully or fail in a controlled manner that preserves system integrity, supports deterministic recovery and provides sufficient diagnostic information to enable rapid investigation and resolution.

Architectural decisions shall favour designs that minimise operational risk, prevent data corruption and make failures observable, even where those designs require additional engineering effort.

### 2. Architectural Guidance

The principle should explicitly reinforce that:

- Correctness takes precedence over implementation convenience.
- Business-critical data shall never be silently lost.
- Partial operations shall leave the system in a recoverable state.
- Failures shall be observable through appropriate logging, telemetry and alerting.
- Systems shall be designed for deterministic recovery wherever practical.
- Reliability considerations shall influence all architectural decisions.

### 3. Related Principles

Review the existing Architectural Principles and update any cross-references where appropriate to ensure consistency with the introduction of AP-000.

No existing principles should be removed unless required solely to eliminate duplication.

### 4. Revision History

Add a new revision history entry describing the introduction of the Reliability over Convenience architectural principle.

## Acceptance Criteria

- AP-000 appears as the first architectural principle.
- Reliability over Convenience is clearly established as the project’s primary engineering philosophy.
- Existing architectural principles remain consistent.
- Document numbering and formatting remain unchanged except where required to accommodate the new principle.
- Revision History updated.

## Review Comment

> **“Bugs are for Lepidopterists.”**

# CR-051 - Align Address Interaction and Failure Flows in HJ-105

## Status

Applied — 17 August 2026

## Target

HJ-105 v3.4 → v3.5 Approved.

## Purpose and Approved Baseline

Propagate CON-006–CON-011 through the Vendor Registration behavioural flow.

## Authorised Changes

Model single/list/refinement search outcomes, prohibit submission before complete selection, bind Trading Location, distinguish semantic and technical failures, permit caller retry with the same reference, and prohibit in-process retry and an Epic 1 circuit breaker.

## Non-goals

No production matching algorithm or HTTP failure mapping is selected.

> This Change Request amends only `HJ-105`. It does not amend any other controlled artefact.

## Verification and Completion

Incomplete Address output is not a success and every failure path has zero Vendor, event or publication effects.

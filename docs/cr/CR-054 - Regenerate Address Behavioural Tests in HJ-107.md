# CR-054 - Regenerate Address Behavioural Tests in HJ-107

## Status

Applied — 17 August 2026

## Target

HJ-107 v1.0 → v1.1 Approved.

## Purpose and Approved Baseline

Regenerate HJ-107 using PR-004 from HJ-106 v1.2 and its approved sources.

## Authorised Changes

Preserve every existing VR ID, resolve CON-009/010 dependency statuses, add VR-ADDRESS-009–015 and remove obsolete VR-BLOCKED-001.

## Non-goals

No executable C# test or unapproved HTTP assertion is generated.

> This Change Request amends only `HJ-107`. It does not amend any other controlled artefact.

## Verification and Completion

The catalogue covers client progression, reference binding, snapshot translation, authorities, technical failure, caller retry and prohibited in-process retry/circuit-breaker behaviour without stable-ID reuse.

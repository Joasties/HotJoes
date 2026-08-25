# CR-049 - Define BusinessAddressSnapshot Fields in HJ-004

## Status

Applied — 17 August 2026

## Target

HJ-004 v2.3 → v2.4 Approved.

## Purpose and Approved Baseline

Propagate CON-008 into the Vendor Domain model without importing Address-owned representations.

## Authorised Changes

Define the seven immutable Vendor-owned BusinessAddressSnapshot fields and the application-adapter translation boundary.

## Non-goals

No Address search, invocation, retry, transport or matching mechanics enter the Domain Model.

> This Change Request amends only `HJ-004`. It does not amend any other controlled artefact.

## Verification and Completion

The Address ownership invariant and optional RecipientOrOrganisationName treatment remain intact.

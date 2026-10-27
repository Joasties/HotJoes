import { expect } from "@playwright/test";
import { createHash } from "node:crypto";
import { execFileSync } from "node:child_process";
import path from "node:path";

const repository = path.resolve(import.meta.dirname, "../../..");
const project = required("HOTJOES_COMPOSE_PROJECT");
const environment = {
  ...process.env,
  HOTJOES_COMPOSE_PROJECT: project,
  HOTJOES_POSTGRES_PASSWORD: required("HOTJOES_POSTGRES_PASSWORD"),
  HOTJOES_EDGE_PORT: required("HOTJOES_EDGE_PORT"),
};

export interface VendorEventEvidence {
  readonly eventId: string;
  readonly eventVersion: number;
  readonly serializedEvent: Uint8Array;
}

export interface ComplianceReceiptEvidence {
  readonly eventId: string;
  readonly eventType: string;
  readonly eventVersion: number;
  readonly serializedEventSha256: string;
}

export function scalar(database: string, sql: string): string {
  return execFileSync("docker", [
    "compose", "-p", project, "exec", "-T", "postgres",
    "psql", "-XAt", "-U", "hotjoes", "-d", database, "-c", sql,
  ], { cwd: repository, env: environment, encoding: "utf8" }).trim();
}

export function vendorIdFor(tradingName: string): string {
  return scalar(
    "hotjoes_vendor",
    `SELECT vendor_id FROM vendor_registrations WHERE trading_name = '${literal(tradingName)}';`,
  );
}

export function vendorEventEvidence(vendorId: string): VendorEventEvidence {
  const result = scalar("hotjoes_vendor", `
    SELECT event_id, event_version, encode(serialized_event, 'hex')
    FROM vendor_registration_outbox
    WHERE vendor_id = '${literal(vendorId)}';
  `);
  const rows = result ? result.split("\n") : [];
  expect(rows, "Exactly one Vendor outbox event must exist for the registered Vendor.")
    .toHaveLength(1);
  const [eventId, eventVersion, serializedEvent] = rows[0].split("|");
  expect(eventId).toMatch(uuidPattern);
  expect(eventVersion).toBe("1");
  expect(serializedEvent).toBeTruthy();
  return {
    eventId,
    eventVersion: Number(eventVersion),
    serializedEvent: Buffer.from(serializedEvent, "hex"),
  };
}

export function complianceReceiptEvidence(eventId: string): ComplianceReceiptEvidence {
  const result = scalar("hotjoes_compliance", `
    SELECT event_id, event_type, event_version, encode(serialized_event_sha256, 'hex')
    FROM compliance_vendor_registered_receipts
    WHERE event_id = '${literal(eventId)}';
  `);
  const rows = result ? result.split("\n") : [];
  expect(rows, "Exactly one durable Compliance receipt must exist for the Vendor event.")
    .toHaveLength(1);
  const [receiptEventId, eventType, eventVersion, serializedEventSha256] = rows[0].split("|");
  return {
    eventId: receiptEventId,
    eventType,
    eventVersion: Number(eventVersion),
    serializedEventSha256,
  };
}

export function sha256(bytes: Uint8Array): string {
  return createHash("sha256").update(bytes).digest("hex");
}

export function communityCounts(vendorId: string): readonly number[] {
  const result = scalar("hotjoes_community", `
    SELECT
      (SELECT count(*) FROM community.community_participations WHERE vendor_id = '${literal(vendorId)}'),
      (SELECT count(*) FROM community.community_participation_outbox o JOIN community.community_participations p USING (community_participation_id) WHERE p.vendor_id = '${literal(vendorId)}'),
      (SELECT count(*) FROM community.community_participation_receipts WHERE vendor_id = '${literal(vendorId)}');
  `);
  return result.split("|").map(Number);
}

export function contactPreference(vendorId: string): string {
  return scalar(
    "hotjoes_community",
    `SELECT contact_preference FROM community.community_participations WHERE vendor_id = '${literal(vendorId)}';`,
  );
}

export function composeLogs(): string {
  return execFileSync(
    "docker", ["compose", "-p", project, "logs", "--no-color"],
    { cwd: repository, env: environment, encoding: "utf8" },
  );
}

export async function eventually(
  assertion: () => void,
  timeout = 30_000,
  description = "Expected asynchronous evidence was not observed",
): Promise<void> {
  let lastObservedError: unknown;
  try {
    await expect.poll(() => {
      try {
        assertion();
        return true;
      } catch (error: unknown) {
        lastObservedError = error;
        return false;
      }
    }, { timeout }).toBe(true);
  } catch {
    const detail = lastObservedError instanceof Error
      ? lastObservedError.message.split("\n", 1)[0]
      : "No assertion detail was available.";
    throw new Error(`${description}. Last observation: ${detail}`);
  }
}

function literal(value: string): string { return value.replaceAll("'", "''"); }

function required(name: string): string {
  const value = process.env[name];
  if (!value) throw new Error(`Required browser-harness setting '${name}' is missing.`);
  return value;
}

const uuidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/;

import { execFileSync } from "node:child_process";
import path from "node:path";

const repository = path.resolve(import.meta.dirname, "../../..");
const project = process.env.HOTJOES_COMPOSE_PROJECT ?? "hotjoes-browser-tests";
const environment = {
  ...process.env,
  HOTJOES_COMPOSE_PROJECT: project,
  HOTJOES_POSTGRES_PASSWORD:
    process.env.HOTJOES_POSTGRES_PASSWORD ?? "browser-tests-only",
  HOTJOES_EDGE_PORT: process.env.HOTJOES_EDGE_PORT ?? "18080"
};

export function scalar(database: string, sql: string): string {
  return execFileSync("docker", [
    "compose", "-p", project, "exec", "-T", "postgres",
    "psql", "-XAt", "-U", "hotjoes", "-d", database, "-c", sql
  ], { cwd: repository, env: environment, encoding: "utf8" }).trim();
}

export function vendorIdFor(tradingName: string): string {
  return scalar(
    "hotjoes_vendor",
    `SELECT vendor_id FROM vendor_registrations WHERE trading_name = '${literal(tradingName)}';`
  );
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
    `SELECT contact_preference FROM community.community_participations WHERE vendor_id = '${literal(vendorId)}';`
  );
}

export function composeLogs(): string {
  return execFileSync(
    "docker", ["compose", "-p", project, "logs", "--no-color"],
    { cwd: repository, env: environment, encoding: "utf8" }
  );
}

export async function eventually(assertion: () => void, timeout = 30_000): Promise<void> {
  const deadline = Date.now() + timeout;
  let failure: unknown;
  while (Date.now() < deadline) {
    try { assertion(); return; } catch (error) { failure = error; }
    await new Promise(resolve => setTimeout(resolve, 250));
  }
  throw failure;
}

function literal(value: string): string { return value.replaceAll("'", "''"); }

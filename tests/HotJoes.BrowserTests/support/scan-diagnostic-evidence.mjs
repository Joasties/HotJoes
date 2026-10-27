import path from "node:path";
import {
  forbiddenDiagnosticValues,
  scanDiagnosticEvidence,
} from "./diagnostic-evidence.mjs";

const forbiddenValues = forbiddenDiagnosticValues();
const roots = ["test-results", "playwright-report"];
let unsafeFileCount = 0;

for (const root of roots) {
  const result = await scanDiagnosticEvidence({
    root: path.resolve(root),
    forbiddenValues,
  });
  unsafeFileCount += result.unsafeFiles.length;
}

if (unsafeFileCount > 0) {
  process.stderr.write(
    `Sensitive diagnostic content detected in ${unsafeFileCount} file(s).\n`,
  );
  process.exitCode = 1;
}

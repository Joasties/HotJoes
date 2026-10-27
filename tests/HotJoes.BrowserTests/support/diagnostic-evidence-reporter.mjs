import path from "node:path";
import {
  forbiddenDiagnosticValues,
  scanDiagnosticEvidence,
} from "./diagnostic-evidence.mjs";

export default class DiagnosticEvidenceReporter {
  async onEnd() {
    const result = await scanDiagnosticEvidence({
      root: path.resolve("test-results"),
      forbiddenValues: forbiddenDiagnosticValues(),
    });

    if (!result.safe) {
      throw new Error(
        `Sensitive diagnostic content detected in ${result.unsafeFiles.length} file(s).`,
      );
    }
  }
}

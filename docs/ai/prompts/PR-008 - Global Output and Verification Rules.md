# HotJoes AI Prompt
## PR-008 - Global Output and Verification Rules

### Objective

Provide one shared set of baseline-discovery, output, file-handoff, verification and human-review rules for HotJoes generation and implementation prompts.

PR-002, PR-004, PR-005, PR-006 and PR-007 shall apply this prompt. They shall define their specialist derivation or implementation behaviour and shall not duplicate or weaken these global rules.

# 1. Authority and Precedence

PR-008 controls output packaging, filenames, source-write boundaries, baseline validation, presentation and common preflight verification.

It does not:

- create architectural authority;
- change the authority hierarchy of a governing prompt;
- approve a concern or artefact;
- change business behaviour;
- replace artefact-specific generation, stable-ID or verification rules; or
- authorize repository changes.

Where rules interact, use this precedence:

1. explicit human instruction for the current task;
2. controlled architectural and business authority identified by the governing prompt;
3. the governing prompt's specialist generation or implementation rules;
4. PR-008 shared output and verification rules; and
5. non-authoritative convenience conventions.

A conflict between PR-008 and a governing prompt shall be reported rather than silently resolved.

# 2. Baseline Manifest

Use `HJ-000 - Current Approved Baseline Manifest.md` as the compact baseline index when it is supplied or available.

HJ-000 is not architectural authority. Before generation:

1. identify the artefacts required by the governing prompt;
2. compare their actual Document ID, controlled filename, internal version and status with HJ-000;
3. report only mismatches, missing authority or pending approved updates;
4. continue silently when the required baseline is aligned; and
5. use the actual controlled artefact as authority if a mismatch exists, subject to the governing prompt's stop rules.

Do not reconstruct an already aligned baseline repeatedly in the user-facing response.

## 2.1 Common Execution Header

At the start of an operation, obtain or derive the following compact execution header:

```yaml
governing_prompt: PR-xxx
operation: concise operation name
generation_mode: Initial Generation | Controlled Regeneration | Verification Only | Implementation Stage
baseline_manifest: HJ-000 current controlled version
authoritative_delta:
  - changed or newly approved source artefact/concern
expected_outputs:
  - controlled artefact or stage output
output_mode: Review Package | Manual File Handoff | Repository Application
source_write_authorized: false
human_gate: exact review or approval required
```

The user may supply only the selector needed for the specialist prompt. Derive discoverable values from HJ-000 and the controlled files rather than asking the user to repeat them. Report an assumption only when it is material or a required value cannot be established safely.

The common header is operational context. It is not a controlled architecture artefact, approval record or substitute for the fuller Execution Context required by PR-006 or PR-007.

# 3. Standard Filenames

For a controlled HotJoes artefact:

- use its exact standard repository filename;
- ensure Version and Status appear only inside controlled artefacts header information;
- do not append version, status, date, `approved`, `draft`, `candidate`, `new`, `latest`, `final`, revision labels or similar text to the filename;
- preserve the controlled Document ID and title; and
- do not rename or recreate an unchanged approved artefact merely because approval has been recorded.

Examples:

```text
HJ-106 - Vendor Registration Service Contract.md
HJ-107 - Vendor Registration Test Catalogue.md
```

For source code:

- use the exact approved primary type name as the filename;
- apply the one-primary-type-per-file rule from HJ-005; and
- do not add proposal or version suffixes to code filenames.

For supporting review material, use a descriptive stable name distinct from the controlled artefact, for example:

```text
HJ-107 Regeneration Summary.md
Changed Artefact Preview Index.md
Verification Report.md
```

# 4. Output Package

Place generated review material in one clearly named output directory for the current operation.

The package shall contain only material required for the current human gate:

- each complete changed controlled artefact under its standard filename;
- any required artefact-specific Change Request;
- one concise change or regeneration summary;
- required comparisons and verification reports; and
- for Manual File Handoff, individually generated code files plus a destination manifest.

Do not create redundant copies of an unchanged approved artefact. Do not package the same controlled content under multiple filenames.

# 5. Source-Write Boundary

Generation is proposal-only unless the human explicitly authorizes application to the controlled repository.

- Do not modify HotJoes source artefacts or code merely because a candidate was generated.
- Do not interpret approval as permission to recreate, rename or recopy an artefact already applied by the human.
- When Manual File Handoff is active, generate files for review and identify their intended destinations; the human performs the copy.
- After the human reports that files were copied, verify the destination before claiming application or running the next applicable verification stage.
- Preserve unrelated user changes.

# 6. Document Control and Approval

- Set internal version and status according to the governing prompt and supplied approval evidence.
- Candidate generation does not itself grant approval.
- A generated artefact based entirely on an already approved decision may carry the publication status authorized by the governing workflow; do not invent `Proposed — not controlled` or another transient status when the workflow requires `Approved`.
- If human approval changes only the gate state and the reviewed file already contains the correct content, version and status, do not generate another substantively identical file.
- Update Revision History once for the controlled change being published.
- Do not add transient conversational approval comments to controlled artefacts unless they convey enduring document history.

# 7. Delta-First Generation

For regeneration:

```text
previous controlled artefact
+ approved source delta
+ governing prompt
= regenerated candidate
```

Apply these rules:

- preserve unaffected content;
- change only content affected by the approved source delta or correction of an identified defect;
- preserve stable IDs according to the owning prompt;
- identify preserved, materially changed, added, retired and unresolved content;
- do not stylistically rewrite unaffected sections; and
- do not regenerate downstream artefacts before their authoritative input is approved unless the governing workflow explicitly requests a proposal-only candidate chain.

# 8. Common Preflight Verification

Before presenting output, verify as applicable:

## 8.1 Baseline

- required sources exist;
- HJ-000 alignment has been checked;
- cited versions and statuses match actual inputs;
- approval evidence is sufficient for the requested mode; and
- unresolved or challenged authority has not been treated as approved.

## 8.2 Document Control

- controlled filename is exact;
- Version and Status appear only inside controlled artefacts header information, not the filename;
- revision history matches the material change;
- Document ID and title are preserved; and
- no unnecessary duplicate candidate exists.

## 8.3 Content Integrity

- all sections required by the governing prompt exist;
- no required section is duplicated;
- Markdown fences and tables are structurally complete;
- examples contain no duplicate fields or malformed structures;
- references and traceability use current source versions;
- obsolete concern states and superseded decisions have been removed where affected;
- no unresolved decision has been silently selected; and
- unaffected material remains materially unchanged.

## 8.4 Catalogue Integrity

Where a test catalogue is generated:

- use only the catalogue's owned ID namespace;
- use the prior controlled catalogue as the sole stable-ID baseline;
- report preserved, changed, added and retired IDs;
- do not reuse retired IDs;
- validate active-entry counts; and
- enforce the HJ-107/HJ-013 responsibility boundary.

## 8.5 Code Integrity

Where code is generated:

- every file has one primary type unless HJ-005 explicitly permits a tightly coupled nested type;
- filename and primary type match;
- namespace and destination project are explicit;
- applicable HJ-005 rules have preflight evidence;
- proposed code is traceable to the selected concern and catalogue obligations; and
- verification commands and expected outcomes are stated.

## 8.6 Links

- every generated file intended for review exists;
- every user-facing local link resolves to that file;
- provide a direct link to each changed controlled artefact and each manually handed-off code file; and
- do not require the reviewer to open an index merely to discover the individual artefact links.

# 9. User-Facing Completion Format

The final response shall be self-contained and concise. It shall provide:

1. the outcome;
2. direct links to every changed artefact or generated code file;
3. a material-change summary;
4. verification findings, including anomalies;
5. confirmation of whether controlled source files were modified;
6. the exact human approval gate; and
7. the next permitted action after approval.

Do not require the user to infer the next process step from an earlier commentary message.

# 10. HJ-000 Maintenance

When the reviewed operation changes an artefact indexed by HJ-000:

- include one updated HJ-000 candidate in the same review package;
- change only the affected manifest entries, Last Updated and Revision History;
- do not make HJ-000 part of a business or architecture decision transaction;
- do not claim the manifest update approves the target artefact; and
- do not update HJ-000 for ordinary implementation source files or PR-006 stage records.

If the human has already applied the approved target artefact, an HJ-000-only synchronization is permitted without recreating the target artefact.

# 11. Efficiency Rules

- Read and validate the compact manifest first.
- Inspect full authoritative sources required for the material decision or generation.
- Use focused searches for unchanged supporting sources after their manifest alignment is confirmed.
- Reuse prior controlled artefacts as regeneration baselines.
- Reuse a validated execution context until one of its recorded baselines or selections changes.
- Permit a larger Approved concern cohort to provide a stable dependency horizon while keeping each implementation slice and human gate small.
- Avoid reproducing unchanged source content in summaries.
- Batch independent read-only discovery and verification where safe.
- Generate only the artefacts required by the current gate.
- Stop at the governing human gate rather than generating later-stage work speculatively.

Efficiency shall never weaken authority validation, human approval, traceability, stable-ID preservation or verification.

# 12. Review Checklist

- [ ] HJ-000 was validated as an index rather than treated as authority.
- [ ] Every controlled artefact uses its standard filename.
- [ ] Version and status appear only inside controlled artefacts header information.
- [ ] No unchanged approved artefact was recreated merely to record approval.
- [ ] Only current-gate outputs were generated.
- [ ] Source-write authorization was respected.
- [ ] Delta-first regeneration preserved unaffected content.
- [ ] Common preflight verification passed or findings were reported.
- [ ] Every generated review file has a direct link.
- [ ] The final response states the exact gate and next permitted action.

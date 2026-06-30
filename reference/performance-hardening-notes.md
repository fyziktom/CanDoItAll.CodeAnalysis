# Performance and Safety Hardening Notes

This project favors measured, bounded hardening over broad micro-optimization. The publishing-prep pass selected the following hot-path and public-safety fixes.

## Dynamic Regex Search

Symbol search regex mode accepts user-provided patterns. Dynamic patterns are now interpreted with a 100 ms timeout and no `RegexOptions.Compiled` startup cost. If a pattern times out while matching a value, the matcher fails closed for the rest of that request instead of retrying the same expensive pattern against every symbol field.

## Source Reads

Document, symbol, and focused-context source reads resolve paths under the analyzed workspace root only. Source files above 2 MB are skipped for public read/excerpt responses. Snapshot facts still preserve metadata about those files; only the source-code payload is bounded.

## Export Writes

Snapshot exports are written only when the prepared export path resolves inside the target snapshot directory. This complements the Web export endpoint's read-time containment check and protects future renderers from accidentally writing outside the snapshot.

## Deferred Optimizations

LINQ and collection allocations remain in rendering and query shaping where they keep the code clearer and were not proven to be a bottleneck by the selected tests. Future throughput work should use scenario or benchmark evidence before replacing those paths.

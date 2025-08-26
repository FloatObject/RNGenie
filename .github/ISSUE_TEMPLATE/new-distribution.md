---
name: "📊 New Distribution"
about: Suggest or implement a new probability distribution for RNGenie
labels: enhancement, good first issue, distribution
---

## Distribution Name
(e.g. Exponential, Poisson, Beta, etc.)

## Description
Briefly describe the distribution and why it’s useful in RNG contexts.

## Implementation Notes
- Should implement `IDistribution<T>`.
- Include tests verifying mean/variance are statistically reasonable.
- Add sample usage in `RNGenie.Samples`.

## References
(Optional) Include links to mathematical definitions or usage in games.

## Checklist
- [ ] Implemented distribution under `RNGenie.Distributions`
- [ ] Added statistical sanity tests
- [ ] Updated Samples to demonstrate usage
- [ ] Added XML documentation

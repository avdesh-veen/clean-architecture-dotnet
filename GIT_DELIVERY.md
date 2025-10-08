# Git Delivery Instructions

Use these steps to add the `docs/code-snippets` folder to the repo and push to GitHub for client delivery.

1. Create a feature branch:

```bash
git checkout -b docs/code-snippets
```

2. Stage and commit the new files:

```bash
git add docs/code-snippets
git commit -m "Add code snippets, patterns, and delivery guide"
```

3. Push the branch and open a PR:

```bash
git push --set-upstream origin docs/code-snippets
```

4. In the PR description include:
- Summary of contents
- Which stakeholders to review (architecture owner, security)
- Any follow-up tasks (add more snippets or language translations)

5. After PR approval, merge to `main` and tag a release if needed.

Optional: Create a small release note for clients referencing `docs/code-snippets/README.md`.

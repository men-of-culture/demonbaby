# menofculture
## Master-branch
**Before pushing to this branch make sure you have:**
1. Merged and tested the feature on test-branch
2. Made a pull request containig the feature you would like to push
3. Above pull request have been reviewed and approved by somebody on the team

## Test-branch
This is our test environment and every feature should be merged to the test branch and be tested before it gets merged into master branch. Git commands to merge feature branch to test:
1. `git checkout test`
2. `git merge BRANCHNAME`

## Feature-branch
Every new change to the source code should start as a feature. Git commands to create feature branch:
1. `git checkout master`
2. `git checkout -b feature-TRELLO TICKET NUMBER`
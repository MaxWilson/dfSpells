git branch -D publish && git checkout -b publish && npm run build && git add . && git commit -m publish && git push -f -u origin publish && git checkout main
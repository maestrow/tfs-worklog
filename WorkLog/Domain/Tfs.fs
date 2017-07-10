module Domain.Tfs

open FSharp.Data

module private Internals = 

    [<Literal>]
    let repositories = """
    {
       "value":[
          {
             "id":"7be12d9c-5ce4-40f5-bfec-0e67828da5fd",
             "name":"RepoName",
             "url":"https://tfs.sample.org/Collection/_apis/git/repositories/7be12d9c-5ce4-40f5-bfec-0e67828da5fd",
             "project":{
                "id":"3833b560-01d8-47b9-9066-979720e8f9b3",
                "name":"ProjectName",
                "description":"Some description",
                "url":"https://tfs.sample.org/Collection/_apis/projects/3833b560-01d8-47b9-9066-979720e8f9b3",
                "state":"wellFormed",
                "revision":2692614
             },
             "defaultBranch":"refs/heads/master",
             "remoteUrl":"https://tfs.sample.org/Collection/ProjectName/_git/RepoName"
          },
       ],
       "count":10
    }
    """

    [<Literal>]
    let commitDetailsJson = """
    {
        "treeId": "19699217ecfd77a558b77df9c8ec8114e5b0facd",
        "push": {
            "pushedBy": {
                "id": "8dc917aa-83b2-486f-8d56-f6c6ae675e85",
                "displayName": "John Doe",
                "uniqueName": "DOMAIN\\login",
                "url": "https://tfs.sample.org/Collection/_apis/Identities/8dc917aa-83b2-486f-8d56-f6c6ae675e85",
                "imageUrl": "https://tfs.sample.org/Collection/_api/_common/identityImage?id=8dc917aa-83b2-486f-8d56-f6c6ae675e85"
            },
            "pushId": 26905,
            "date": "2017-06-06T15:30:00.9880083Z"
        },
        "commitId": "2cb7b1ed56e60dabe63a0778041a309301b40d2d",
        "author": {
            "name": "John Doe",
            "email": "login@sample.com",
            "date": "2017-06-06T15:29:31Z"
        },
        "committer": {
            "name": "John Doe",
            "email": "login@sample.com",
            "date": "2017-06-06T15:29:31Z"
        },
        "comment": "Merge branch 'Branch_Name-7.5.1' of https://tfs.sample.org/Collection/_git/ProjectName into Branch_Name-7.5.1",
        "parents": ["6c92f13d38473cb7634a459ea5958adf5986a043", "e80d4bba4b8e141ab8b58ac4b7220f81676492df"],
        "url": "https://tfs.sample.org/Collection/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2/commits/2cb7b1ed56e60dabe63a0778041a309301b40d2d",
        "remoteUrl": "https://tfs.sample.org/Collection/_git/ProjectName/commit/2cb7b1ed56e60dabe63a0778041a309301b40d2d",
        "_links": {
            "self": {
                "href": "https://tfs.sample.org/Collection/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2/commits/2cb7b1ed56e60dabe63a0778041a309301b40d2d"
            },
            "repository": {
                "href": "https://tfs.sample.org/Collection/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2"
            },
            "changes": {
                "href": "https://tfs.sample.org/Collection/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2/commits/2cb7b1ed56e60dabe63a0778041a309301b40d2d/changes"
            },
            "web": {
                "href": "https://tfs.sample.org/Collection/_git/ProjectName/commit/2cb7b1ed56e60dabe63a0778041a309301b40d2d"
            },
            "tree": {
                "href": "https://tfs.sample.org/Collection/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2/trees/19699217ecfd77a558b77df9c8ec8114e5b0facd"
            }
        }
    }
    """

    [<Literal>]
    let commitsJson = """
    {
        "count": 100,
        "value": [{
            "commitId": "2cb7b1ed56e60dabe63a0778041a309301b40d2d",
            "author": {
                "name": "John Doe",
                "email": "login@sample.com",
                "date": "2017-06-06T15:29:31Z"
            },
            "committer": {
                "name": "John Doe",
                "email": "login@sample.com",
                "date": "2017-06-06T15:29:31Z"
            },
            "comment": "Merge branch 'Branch_Name-7.5.1' of https://tfs.sample.org/Collection/_git/ProjectName into N",
            "commentTruncated": true,
            "changeCounts": {
                "Add": 3,
                "Edit": 13
            },
            "url": "https://tfs.sample.org/Collection/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2/commits/2cb7b1ed56e60dabe63a0778041a309301b40d2d",
            "remoteUrl": "https://tfs.sample.org/Collection/_git/ProjectName/commit/2cb7b1ed56e60dabe63a0778041a309301b40d2d"
        }, {
            "commitId": "6c92f13d38473cb7634a459ea5958adf5986a043",
            "author": {
                "name": "John Doe",
                "email": "login@sample.com",
                "date": "2017-06-06T15:27:53Z"
            },
            "committer": {
                "name": "John Doe",
                "email": "login@sample.com",
                "date": "2017-06-06T15:29:21Z"
            },
            "comment": "Fixed error on parsing date in IE\n\nRelated Work Items: #21409",
            "changeCounts": {
                "Edit": 1
            },
            "url": "https://tfs.sample.org/Collection/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2/commits/6c92f13d38473cb7634a459ea5958adf5986a043",
            "remoteUrl": "https://tfs.sample.org/Collection/_git/ProjectName/commit/6c92f13d38473cb7634a459ea5958adf5986a043"
        }]
    }
    """

open Internals

type TfsCommitDetails = JsonProvider<commitDetailsJson>

type TfsCommits = JsonProvider<commitsJson>


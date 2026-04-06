export interface BuildInfo {
  commitSha: string
  shortSha: string
  branch: string
  buildTimestamp: string
  repositoryUrl: string
  commits: Commit[]
}

export interface Commit {
  sha: string
  shortSha: string
  message: string
  author: string
  date: string
}

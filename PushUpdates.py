import subprocess

commit_description = input("commit description: ")

subprocess.run(["git", "add", "."])
subprocess.run(["git", "commit", "-m", commit_description])
subprocess.run(["git", "push"])

x = input("press enter to finish")

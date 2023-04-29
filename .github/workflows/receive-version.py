import re
import os
from pathlib import Path

# read text from file containing version
filePath = Path(__file__).parent.parent.parent / "src" / "build" / "AppxManifest.xml"

print(f"Reading Version from '{filePath}'")

with open(filePath, "r") as f:
    data = f.read()

# parse version out of the file
def parse(data=data):
    output = re.search('<Identity (.*?) Version=\"(?P<Version>(.*?))\"', data, flags=re.X)
    return output.group('Version')

# set github enviorment variable
versionName = parse()
print(f"Version: '{versionName}'")

env_file = os.getenv('GITHUB_ENV')

with open(env_file, "a") as myfile:
    myfile.write(f"Release_Name={versionName}")
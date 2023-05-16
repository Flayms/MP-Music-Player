import re
import os
import glob
from datetime import datetime

### reads the application-version number from the windows build and writes it into github enviornment variable "Release_Name"

# read version from csproj
filePath = glob.glob("**/*.csproj")[0]

print(f"Reading Version from '{filePath}'")

with open(filePath, "r") as f:
    data = f.read()

# parse version out of the file
def parse(data=data):
    output = re.search('<ApplicationDisplayVersion>(?P<Version>(.*?))</ApplicationDisplayVersion>', data, flags=re.X)
    return output.group('Version')

versionName = parse()

# combine with datetime
currentTime = datetime.utcnow()
timeForTag = currentTime.strftime("%Y%m%dT%H%M")
timeForRelease = currentTime.strftime("%Y-%m-%d %H:%M")

tagName = f"v{versionName}-{timeForTag}" 
releaseName = f"Version {versionName} ({timeForRelease})" 
applicationVersion = versionName.replace(".", "")

print(f"Tag_Name:     {tagName}")
print(f"Release_Name: {releaseName}")
print(f"Application_Version: {applicationVersion}")

# set github enviorment variable
env_file = os.getenv('GITHUB_ENV')

with open(env_file, "a") as myfile:
    myfile.write(f"Tag_Name={tagName}\r\n")
    myfile.write(f"Release_Name={releaseName}\r\n")
    myfile.write(f"Application_Version={applicationVersion}")
import re

#todo: need to write new commit-message-file

message_reg = r"(P<message>(?P<starter>(\+|\*|-|#)(?P<content>(.)+?))(?=\+|\*|-|#|\r|\n|\r\n|$))"
messages = []

with open("../../new-in-this-release.log", 'r') as messages_log:
	for line in messages_log:
		matches = re.findall(message_reg, line)
		someMatch = re.search(message_reg, line)
		print(someMatch)
	
		for match in matches:
			print("hello " + match)
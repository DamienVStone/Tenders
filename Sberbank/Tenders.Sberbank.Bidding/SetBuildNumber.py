import sys

filePath, buildNumber = sys.argv[1:3]
f = open(filePath,'r')
filedata = f.read()
f.close()
newdata = filedata.replace("{{BUILDNUMBER}}", buildNumber)
f = open(filePath,'w')
f.write(newdata)
f.close()
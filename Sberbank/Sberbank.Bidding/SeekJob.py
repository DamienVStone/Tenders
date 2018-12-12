import sys
import requests

jobFilePath, seekUrl = sys.argv[1:3]
def readFile():
	f = open(jobFilePath,'r')
	t = f.read()
	f.close()
	return t

def findJob():
	try:
		result = requests.get(seekUrl).json()
		return readFile().replace("{{REGNUMBER}}", result["regNumber"]).replace("{{WORKERS}}", str(result["workers"])).replace("{{AUCTION_JSON}}", result)
	except Exception:
		return ""

print(findJob())
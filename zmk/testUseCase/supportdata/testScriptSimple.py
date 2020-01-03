import time
import sys
import ast,json
def testCounter(upto):
	# upto = ast.literal_eval(upto)
	for i in range(upto):
		print(i)
		time.sleep(2)

def justTest(name,age):
	print("name ",name," age ",age+1)
        


print (justTest('Swapnil',7))

#class my_class(object):
#    pass 
#2+3		#didn't display anything
import os
from azure.ai.formrecognizer import DocumentAnalysisClient #has intellisense/autocomplete-yay!
from azure.core.credentials import AzureKeyCredential
#print(2+4)	#prints 6 - yay!
#print (os.getenv("formRecog_key"))
key = os.getenv("formRecog_key")
endpoint = os.getenv("formRecog_url")
def analyze_layout():
	formUrl = ("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples"
		"/master/curl/form-recognizer/sample-layout.pdf")
	document_analysis_client = DocumentAnalysisClient(
		endpoint=endpoint, credential=AzureKeyCredential(key)
	)
	poller = document_analysis_client.begin_analyze_document_from_url(
		"prebuilt-layout",formUrl
	)
	result = poller.result()
	print ("Dave's jumpy-up-and-downy Azure Doc Intel endpoint URLy thingy: {}".format(endpoint))
	#print("# of tables: {}".format(result.tables.count()))   error
	print("The groovy-2-shoes # of tables: {}".format(len(result.tables))) #2
	#print("# rows in 1st tbl: {}".format(result.tables[0].row_count)) #5
	print("The 1st tbl has {} rows and {} columns".format(result.tables[0].row_count, 
		result.tables[0].column_count)) #The 1st tbl has 5 rows and 3 columns
	polygonPoints = ""
	wordCnt = 0
	for ξ in result.tables[0].cells:
		print("({},{}) contains: {}".format(ξ.row_index, ξ.column_index, ξ.content))
		for み in ξ.bounding_regions:
			#print(" -- # points in polygon {}".format(len(み.polygon))) #4
			polygonPoints = "     points in the cell's kick-ass polygon: "
			for ろ in み.polygon:
				#print("({},{})  ".format(ろ.x, ろ.y))
				polygonPoints += "({},{})  ".format(ろ.x, ろ.y)
			print(polygonPoints)
	for page in result.pages:
		print(
			"Page {} has width {} and height {} in {} units".format(
				page.page_number, page.width,page.height,page.unit))
		for line_idx, line in enumerate(page.lines):
			if len(line.get_words())>4 and len(line.get_words())<10:
				print("...Line #{} has word count {} and text:: '{}'".format(
					line_idx, len(line.get_words()), line.content) )
				wordCnt=1
				for め in line.get_words():
					print("      {}.{}   confidence:{} ".format(wordCnt, め.content, め.confidence))
					wordCnt+=1
if __name__ == "__main__":
	analyze_layout()
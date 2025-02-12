const DocumentIntelligence = require("@azure-rest/ai-document-intelligence").default,
    { getLongRunningPoller, isUnexpected } = require("@azure-rest/ai-document-intelligence");
const { AzureKeyCredential } = require("@azure/core-auth");
// set `<your-key>` and `<your-endpoint>` variables with the values from the Azure portal.
const key = process.env.formRecog_key;
const endpoint = process.env.formRecog_url;
// sample document
const invoiceUrl = "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-invoice.pdf"
async function main() {
    const client = DocumentIntelligence(endpoint, new AzureKeyCredential(key));
    const initialResponse = await client
        .path("/documentModels/{modelId}:analyze", "prebuilt-invoice")
        .post({
            contentType: "application/json",
            body: {
                // The Document Intelligence service will access the URL to the invoice image and extract data from it
                urlSource: invoiceUrl,
            },
        });
    if (isUnexpected(initialResponse)) {
        throw initialResponse.body.error;
    }
    const poller = await getLongRunningPoller(client, initialResponse);
    //poller.onProgress((state) => console.log("Operation:", state.result, state.status));
    //const analyzeResult = (await poller.pollUntilDone()).body.analyzeResult;
    const ar = (await poller.pollUntilDone()).body.analyzeResult;
    const documents = ar?.documents;
    //console.log("# of fields:", ar?.documents[0].fields.length); //undefined
    arp0 = ar?.pages[0];
    /*console.log("The boogie page:");
    console.log(ar?.pages); //shows page#, W, H, ... [Object] for lines and words, for 1 page */
    //console.log(analyzeResult?.pages[0].content); //undefined
    console.log("Page width-a-doodle-don't: ", arp0.width); //8.5
    console.log("Page height-a-doodle-doo", arp0.height); //11
    console.log("word count: ", arp0.words.length); //115
    //arp0.words[0].boundingPolygon  undefined
    //arp0.words[0].boundingRegions.length      undefined
    console.log("1st word: ", arp0.words[0].content, "  confidence:",
        arp0.words[0].confidence); //CONTOSO .998  
    console.log("line count: ", arp0.lines.length); //55 
    console.log("1st line: ", arp0.lines[0].content); //CONTOSO LTD.
    for (var ln = 0; ln < 10; ln++)
        //arp0.lines[ln].boundingPolygon undefined
        console.log(ln, ": ", arp0.lines[ln].content, arp0.lines[ln].confidence); //arp0.lines[ln].boundingBox undefined
    //console.log(analyzeResult?.tables); //shows row count, column count, bounding, ... for 3 tbls
    //displays: No key value pairs:
    if (ar?.keyValuePair) { 
        console.log(ar?.keyValuePair);
    } else {
        console.log("No key value pairs"); 
    }
    /*Cannot read properties of undefined (reading 'length'): */
    console.log("# of table, Jabroni", ar?.tables.length); //3
    console.log("Yo playa, the number of cells in the 1st table is:", ar?.tables[0].cells.length); //12
    let brianBrombergsBombasticInt = 0;
    for (const antame of ar?.tables) {
        console.warn("# of cells in table", brianBrombergsBombasticInt, ":", antame.cells.length);
        brianBrombergsBombasticInt++;
    } //works
    console.log("Hey Jabroni, the number of columns in the 1st table is:", ar?.tables[0].columnCount
        , " and the number of rows is", ar?.tables[0].rowCount); //6  2
    //ar?.tables[0].confidence      undefined
    console.log("table bounding regions: ", ar?.tables[0].boundingRegions.length, ); //1
    //console.log("table bounding : ", ar?.tables[0].boundingRegions[0].boundingPolygon.length); //error
    console.log("table boundingRegions polygon: ", ar?.tables[0].boundingRegions[0].polygon); //4 points
    console.log("table boundingRegions polygon 0: ", ar?.tables[0].boundingRegions[0].polygon[0]); // 0.4823
    console.log("table boundingRegions polygon length: ", ar?.tables[0].boundingRegions[0].polygon.length); //8
    console.log("The party-on points in the polygon are:");
    for (horsePotato = 0; horsePotato < ar?.tables[0].boundingRegions[0].polygon.length; horsePotato += 2) //horsePotato += 2 - error
        console.log("(", ar?.tables[0].boundingRegions[0].polygon[horsePotato], ",",
            ar?.tables[0].boundingRegions[0].polygon[horsePotato + 1] + ")");
    //console.log("Hey Jethro, # rows in 1st table:", ar?.tables[0].rows.length); //error
    //ar?.tables[0].cells.forEach(lmn => { console.log(lmn.row_index, ","); }); //12 undefined
    for (const cell of ar?.tables[0].cells)
        console.log("(", cell.rowIndex, ",", cell.columnIndex, ")", cell.content); //12 works
    const result = documents && documents[0];
    if (result) {
        console.log("result is AOK");
        //console.log(result.fields[1].content.value); //error
        //console.log(result.fields);  //works - AmountDue, BillingAddress, ...
        console.log(result.fields["AmountDue"]);  //type, valueCurrency, content, ...
        console.log("AmountDue's content:", result.fields["AmountDue"].content,
            "  type:", result.fields["AmountDue"].type, "  currencySymbol:",
            result.fields["AmountDue"].valueCurrency.currencySymbol, "  currencyCode:"
            , result.fields["AmountDue"].valueCurrency.currencyCode, "  confidence:"
            , result.fields["AmountDue"].confidence);
        console.log("AmountDue's bounding polygon's 1st coordinate: "
            , result.fields["AmountDue"].boundingRegions[0].polygon[0]);
        console.log("AmountDue's bounding polygon's coordinates: ");
        let strPoly = "   ";
        for (poly = 0; poly < result.fields["AmountDue"].boundingRegions[0].polygon.length; poly += 2)
            strPoly += "(" + result.fields["AmountDue"].boundingRegions[0].polygon[poly] + "," +
                result.fields["AmountDue"].boundingRegions[0].polygon[poly + 1] + ")  "  ;
        console.log(strPoly);
        //const keysPlease = Object.keys(result.fields); //no error
        //const keysPlease = Object.keys(documents[0].fields); //no error
        const keysPlease = Object.keys(ar.documents[0].fields);
        //console.log("1st fields key: ", keysPlease[0]); //works
        const valueCity = Object.values(ar.documents[0].fields);
        console.log(keysPlease[0], ":", valueCity[0]);
        console.log("Funky Cold Medina says the offset is: ", valueCity[0].spans[0].offset
            , " and the party-on amount is ", valueCity[0].valueCurrency.amount);
        let keyWest = "";
        for (const key of keysPlease)
            keyWest += key + ", "; 
        console.log("The boogie-get-sideways invoice fields: ", keyWest);
        console.log(keysPlease[1], ":", valueCity[1]);
        console.log(keysPlease[1], "'s valueAddress.streetAddress:"
            , valueCity[1].valueAddress.streetAddress);
        for (const key of keysPlease)
            console.log(key + " - " + ar.documents[0].fields[key].type);
        console.log("The runny-all-aroundy date fields:");
        for (const key of keysPlease)
            if (ar.documents[0].fields[key].type == "date")
                console.log(key, "-", ar.documents[0].fields[key].valueDate);
        console.log("The shang-hai-shek Items field:");
        console.log(ar.documents[0].fields["Items"]);
        console.log("The jumpy-up-&-downy content in Items:");
        console.log(ar.documents[0].fields["Items"].valueArray[0].content);
        console.log("1st bounding region:", ar.documents[0].fields["Items"].valueArray[0].boundingRegions[0]);
        console.log("# of bounding regions:", ar.documents[0].fields["Items"].valueArray[0].boundingRegions.length); //1
        console.log("1st bounding region's page #:", ar.documents[0].fields["Items"].valueArray[0].boundingRegions[0].pageNumber);
        console.log("# of items in 1st bounding region's polygons:",
            ar.documents[0].fields["Items"].valueArray[0].boundingRegions[0].polygon.length); //8
        strPoly = "The party-on polygon point: ";
        for (ψ = 0; ψ < ar.documents[0].fields["Items"].valueArray[0].boundingRegions[0].polygon.length; ψ += 2)
            strPoly += "(" + ar.documents[0].fields["Items"].valueArray[0].boundingRegions[0].polygon[ψ] + ","
                + ar.documents[0].fields["Items"].valueArray[0].boundingRegions[0].polygon[ψ + 1] + ")  ";
        console.log(strPoly); //(0.5825,6.0793)  (7.93,6.0704)  (7.9302,6.2368)  (0.5827,6.2456)
        //console.log("2/2:", 2 / 2); //1
        strPoly = "The party-on polygon numbered points: ";
        for (ψ = 2; ψ <= ar.documents[0].fields["Items"].valueArray[0].boundingRegions[0].polygon.length; ψ += 2)
            strPoly += ψ/2 + ":(" + ar.documents[0].fields["Items"].valueArray[0].boundingRegions[0].polygon[ψ-2] + ","
                + ar.documents[0].fields["Items"].valueArray[0].boundingRegions[0].polygon[ψ - 1] + ")  ";
        console.log(strPoly); //1:(0.5825,6.0793)  2:(7.93,6.0704)  3:(7.9302,6.2368)  4:(0.5827,6.2456)
        console.log("# of spans:", ar.documents[0].fields["Items"].valueArray[0].spans.length); //1
        console.log("1st spans:", ar.documents[0].fields["Items"].valueArray[0].spans[0]);
        console.log("The rockin' span's offset: " + ar.documents[0].fields["Items"].valueArray[0].spans[0].offset
            + " and it's party-on-Garth length:", ar.documents[0].fields["Items"].valueArray[0].spans[0].length); //539 30
        console.log("The holy-haberdashery confidence of Items is",
            ar.documents[0].fields["Items"].valueArray[0].confidence); // 0.923
        /*Cannot read properties of undefined (reading 'content'), I think since fields 
        is a dictionary, which seems to need text indexes instead of int:
        console.log("The boogie-get-down 1st field's content: ", result.fields[0].content); */
        //console.log(result.fields[0]);  //ReferenceError: document is not defined
        //console.log(result.fields[1]); //ReferenceError: document is not defined
    } else {
        //throw new Error("Expected at least one invoice in the result.");
        throw new Error("Ordinary result this ain't since it seems to be false");
    }
    /* //Extracted invoice: undefined (confidence: <undefined>) : 
    console.log(
        "Extracted invoice:",
        documents.docType,
        `(confidence: ${documents.confidence || "<undefined>"})`,
    );      */
    //console.log("Fields:", documents.fields);   // Fields: undefined
}
main().catch((error) => {
    console.error("A grungy, no-fun error occurred:", error);
    process.exit(1);
});

//This is a function that goes into the 'methods' section of a vue.js page component.
//the code will get data from a webservice provided in the 'theurl' field.
//the result from the web service is assigned to this.data
//this assumes that in the 'data' section of the component is a property called 'data'

getDataFromWebservice() {
    // Make an API request and populate the 'data' property
    let theurl = "";
    fetch(theurl)
        .then(response => response.json())
        .then(data => {
            this.data = data; // Populate the 'data' property with the fetched data
        })
        .catch(error => {
            console.error('Error fetching details:', error);
        });
},

//async send to service
async SendToService(consentObject) {
      const jsonData = JSON.stringify(consentObject)
  //the options for the post
      const options = {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: jsonData
      }

      try {
        const response = await fetch(
          'url-to-webservice',
          options
        )
        if (!response.ok) {
          throw new Error(`Error posting data: ${response.statusText}`)
        } else {
          this.savedconsent = true
          this.haserror = false
        }
        // Process the response data as needed
      } catch (error) {
        this.haserror = true
        this.errormessage = error
      }
    }

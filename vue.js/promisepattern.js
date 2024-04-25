// En funktion, der returnerer en promise
function fetchData() {
  return new Promise((resolve, reject) => {
    // Simulerer en asynkron HTTP-anmodning
    setTimeout(() => {
      const data = 'Data fra serveren';
      if (data) {
        resolve(data); // Promise opfyldt med data
      } else {
        reject('Fejl: Data kunne ikke hentes'); // Promise afvist med en fejlmeddelelse
      }
    }, 2000);
  });
}

// Brug af promises til at håndtere asynkron datahentning
fetchData()
  .then(data => {
    console.log('Data modtaget:', data);
  })
  .catch(error => {
    console.error('Fejl:', error);
  });

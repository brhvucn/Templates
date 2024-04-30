function generateCSharpClassesFromJSON(json, rootClassName = 'RootObject') {
    // Parse the JSON
    const parsedJSON = JSON.parse(json);

    // Object to store all classes
    const classes = {};

    // Generate classes recursively
    generateCSharpClass(parsedJSON, rootClassName, classes);

    // Return all classes joined as a single string
    return Object.values(classes).join('\n\n');
}

function generateCSharpClass(data, className, classes) {
    // Start the class string
    let classString = `public class ${className}\n{\n`;
    const properties = [];

    // Iterate over each property in the data
    for (const [key, value] of Object.entries(data)) {
        // Check if the value is an array
        if (Array.isArray(value)) {
            // Check if the array contains objects
            if (value.length > 0 && typeof value[0] === 'object') {
                // Generate a class for the inner objects
                const innerClassName = capitalizeFirstLetter(key);
                generateCSharpClass(value[0], innerClassName, classes);
                // Create a property with the inner class type
                properties.push(`    public List<${innerClassName}> ${key} { get; set; }`);
            } else {
                // Create a property with the array element type
                properties.push(`    public List<${getDataType(value[0])}> ${key} { get; set; }`);
            }
        } else if (typeof value === 'object' && value !== null) {
            // Generate a class for the nested object
            const innerClassName = capitalizeFirstLetter(key);
            generateCSharpClass(value, innerClassName, classes);
            // Create a property with the nested class type
            properties.push(`    public ${innerClassName} ${key} { get; set; }`);
        } else {
            // Create a property with the primitive type
            properties.push(`    public ${getDataType(value)} ${key} { get; set; }`);
        }
    }

    // Join all properties and add them to the class string
    classString += properties.join('\n') + '\n}\n';

    // Store the class string in the classes object
    classes[className] = classString;
}

// Function to determine data type
function getDataType(value) {
    if (typeof value === 'string') {
        if (value.match(/\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2} \+\d{2}:\d{2}/)) {
            return 'DateTime';
        }
        return 'string';
    } else if (typeof value === 'number') {
        return Number.isInteger(value) ? 'int' : 'double';
    } else if (typeof value === 'boolean') {
        return 'bool';
    } else {
        return 'object';
    }
}

// Function to capitalize the first letter of a string
function capitalizeFirstLetter(string) {
    // This is a placeholder, replace it with your logic
    return string.charAt(0).toUpperCase() + string.slice(1);
}

mergeInto(LibraryManager.library, {
    // Function example
    SetTestToken: function () {
        // Show a message as an alert
        window.localStorage.setItem('jwt_student', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoic3R1ZGVudCIsInVzZXJfaWQiOiJmMDFlY2VjZC00YjhlLTQ4ODctOWYwNi0xZjE0NmUxN2VlNGIiLCJuYW1lIjpudWxsLCJpYXQiOjE2NjU0OTE3NTYsImV4cCI6MTY2NTU3ODE1NiwiYXVkIjoicG9zdGdyYXBoaWxlIiwiaXNzIjoicG9zdGdyYXBoaWxlIn0.1Bwq67KkgZbw63HmEOSjUXgatf8oXpozHsiRSl9wA50');
    },
    // Function with the text param
    PassTextParam: function (text) {
        // Convert bytes to the text
        var convertedText = UTF8ToString(text);
        console.log(convertedText);
    },
    // Function returning text value
    GetToken: function () {
        // Define text value
        var token = window.localStorage.getItem('jwt_student');
        console.log(token)
        // Create a buffer to convert text to bytes
        var bufferSize = lengthBytesUTF8(token) + 1;
        var buffer = _malloc(bufferSize);
        // Convert text
        stringToUTF8(token, buffer, bufferSize);
        // Return text value
        return buffer;
    },
});
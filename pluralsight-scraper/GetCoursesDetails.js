() => {
    const selectors = Array.from(document.querySelectorAll("div.css-kxulf3 a"));

    return selectors.map(s => {
        const courseName = s.innerText;
        const subSelectors = Array.from(s.parentNode.parentNode.parentNode.parentNode.parentNode.querySelectorAll("span.css-1kcrbi9"));
        const isPluralsightPath = subSelectors.length === 2;

        if (isPluralsightPath) {
            return {
                name: courseName,
                level: "${pluralsightPathLevel}",
                datePublished: ""
            }
        }

        const isPluralsightCourse = subSelectors.length === 5;

        if (isPluralsightCourse) {
            return {
                name: courseName,
                level: subSelectors[2].innerText,
                datePublished: subSelectors[3].innerText
            }
        }

        return {
            name: courseName,
            level: "unknown",
            datePublished: ""
        }
    });
}

const comps = [];
async function addComponent(axios, defineComponent, componentName, hasBody, hasScript, domUrl, scriptUrl) {
    if (hasBody.toLowerCase() === 'true') {
        const domResponse = await axios.get(domUrl);
        if (domResponse.status == 200 && domResponse.data) {
            let comp = defineComponent({ name: componentName, template: domResponse.data });
            if (hasScript.toLowerCase() === 'true') {
                const scriptResponse = await axios.get(scriptUrl);
                if (scriptResponse.status == 200 && scriptResponse.data) {
                    const rootString = `<root>${scriptResponse.data}</root>`;
                    const domParser = new DOMParser();
                    var dom = domParser.parseFromString(rootString, "text/html");
                    var tags = dom.querySelector('root').getElementsByTagName("script");
                    if (tags && tags.length) {
                        let innerText = '';
                        for (const tag of tags) {
                            innerText += tag.innerText;
                        }
                        var newComp = await import(URL.createObjectURL(new Blob([innerText], { type: 'application/javascript' })));
                        if (newComp && newComp.default) {
                            comp = defineComponent({ ...newComp.default, name: componentName, template: domResponse.data });
                        }
                    }
                }
            }
            return comp;
        }
    }
}
console.log("hello_custom");

class InlineSVG extends HTMLElement {
    constructor() {
        super();
    };

    connectedCallback() {
        console.log(this);
        let l_src = this.getAttribute("src");
        const l_http = new XMLHttpRequest();
        l_http.open("GET", l_src);
        l_http.send();
        l_http.onreadystatechange = () => {
            if (l_http.readyState === XMLHttpRequest.DONE) {
                if (l_http.status >= 200 && l_http.status < 400) {
                    this.innerHTML = l_http.responseText;
                }
            }
        };
    };
};

customElements.define('svg-inline', InlineSVG);
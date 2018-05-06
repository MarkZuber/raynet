import React from 'react';
import ReactDOM from 'react-dom';
import App from 'views/App';
import Dashboard from 'views/Dashboard';

// Import all the routeable views into the global window variable.
Object.assign(window, {
    Dashboard,
});

// Hot module replacement.  
if (module.hot) {
    const render = (react, elemId) => {
        ReactDOM.unmountComponentAtNode(document.getElementById(elemId));
        ReactDOM.render(React.createElement(react), document.getElementById(elemId));
    }

    module.hot.accept('./views/App.js', _ => render(require('views/App').default, 'App'));
    module.hot.accept('./views/Dashboard.js', _ => render(require('views/Dashboard').default, 'Content'));
}

export default App;
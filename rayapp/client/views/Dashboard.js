import React from 'react';
import dotnetify from 'dotnetify';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import ImageRender from '../components/dashboard/ImageRender';
import globalStyles from '../styles/styles';
import ThemeDefault from '../styles/theme-default';
import auth from '../auth';

class Dashboard extends React.Component {

  constructor(props) {
    super(props);
    this.vm = dotnetify.react.connect("Dashboard", this, {
      exceptionHandler: ex => {
         alert(ex.message);
         auth.signOut();
      }
    });
    this.dispatch = state => this.vm.$dispatch(state);

    this.state = {
      ImageRender: [],
    };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <MuiThemeProvider muiTheme={ThemeDefault}>
        <div>
          <h3 style={globalStyles.navigation}>Application / Dashboard</h3>

          <div className="row">
            <div className="col-xs-12 col-sm-6 col-md-6 col-lg-6 col-md m-b-15">
              <ImageRender data={this.state.ImageRender} />
            </div>
          </div>

        </div>
      </MuiThemeProvider>
    );
  }
}

export default Dashboard;

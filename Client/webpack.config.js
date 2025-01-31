const path = require('path');

module.exports = {
    mode: 'development',
    entry: './Client/Assets/Scripts/Test.ts',
    output: {
        filename: 'bundle.js',
        path: path.resolve(__dirname, './Client/wwwroot/js/dist'),
        library: '___MyScripts',
        libraryTarget: 'var',
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
            {
                test: /\.m?js$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env'],
                    },
                },
            },
        ],
    },
};